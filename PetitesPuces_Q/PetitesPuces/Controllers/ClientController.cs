using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.Utilities;
using PetitesPuces.ViewModels;
using PetitesPuces.ViewModels.Vendeur;
using Rotativa;

namespace PetitesPuces.Controllers
{
    [Securise(RolesUtil.CLIENT)]
    public class ClientController : Controller
    {
        private readonly long NOCLIENT = SessionUtilisateur.NoUtilisateur ?? -1;
        private const int DEFAULTITEMPARPAGE = 8;
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index(string Status = "")
        {
            ViewBag.Status = Status;
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);
            List<PPCommande> lstCommandes = GetCommandesClient(NOCLIENT);

            AccueilViewModel viewModel = new AccueilViewModel
            {
                Paniers = lstPaniers,
                Commandes = lstCommandes
            };
            return View(viewModel);
        }

        private void CreateVisiteVendeur(int noVendeur)
        {
            context.PPVendeursClients.InsertOnSubmit(
                new PPVendeursClient
                {
                    NoClient = NOCLIENT,
                    NoVendeur = noVendeur,
                    DateVisite = DateTime.Now
                });

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private CatalogueViewModel GetCatalogueViewModel(ref IEnumerable<PPProduit> listeProduits,
            string Vendeur, string Categorie, string Page, string Size,
            string Filtre, Tri tri)
        {
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;

            //chercher la categorie
            PPCategory categorie;
            int NoCategorie;
            if (String.IsNullOrEmpty(Categorie) || !int.TryParse(Categorie, out NoCategorie))
            {
                categorie = null;
            }
            else
            {
                var requeteCategorie = (from uneCategorie in context.PPCategories
                    where uneCategorie.NoCategorie == NoCategorie
                    select uneCategorie);
                categorie = requeteCategorie.FirstOrDefault();
            }

            //chercher le vendeur
            PPVendeur vendeur;
            int NoVendeur;
            if (Vendeur == "-1" || String.IsNullOrEmpty(Vendeur) || !int.TryParse(Vendeur, out NoVendeur))
            {
                vendeur = new PPVendeur
                {
                    NoVendeur = -1
                };

                //creer la liste de produits
                listeProduits = (from p in context.PPProduits.Where(p => p.Disponibilité == true) select p)
                    .Where(p => categorie == null || p.PPCategory == categorie);
            }
            else
            {
                var requete = (from unVendeur in context.PPVendeurs
                    where unVendeur.NoVendeur == NoVendeur
                    select unVendeur);
                vendeur = requete.FirstOrDefault();
                CreateVisiteVendeur((int) vendeur.NoVendeur);
                //creer la liste de produits
                listeProduits = vendeur.PPProduits.Where(p => p.Disponibilité == true)
                    .Where(p => (categorie == null || p.PPCategory == categorie) && p.Disponibilité == true);
            }

            if (!String.IsNullOrEmpty(Filtre))
                listeProduits = listeProduits.Where(p => p.Nom.ToLower().Contains(Filtre.ToLower()));

            switch (tri)
            {
                case Tri.Nom:
                    listeProduits = listeProduits.OrderBy(p => p.Nom);
                    break;
                case Tri.Categorie:
                    listeProduits = listeProduits.OrderBy(p => p.PPCategory.Description);
                    break;
                case Tri.Date:
                    listeProduits = listeProduits.OrderBy(p => p.DateCreation);
                    break;
                case Tri.Numero:
                    listeProduits = listeProduits.OrderBy(p => p.NoProduit);
                    break;
                default:
                    listeProduits = listeProduits.OrderBy(p => p.Nom);
                    break;
            }

            //creer le view model
            var viewModel = new CatalogueViewModel
            {
                Vendeur = vendeur,
                Vendeurs = (from unVendeur in context.PPVendeurs
                    select unVendeur).ToList(),
                Categories = (from uneCategorie in context.PPCategories
                    select uneCategorie).ToList(),
                Categorie = categorie,
                Produits = listeProduits.Skip(noPage * (nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage) -
                                              (nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage))
                    .Take((nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage)).ToList()
            };
            return viewModel;
        }

        public ActionResult Catalogue(string Vendeur, string Categorie, string Page, string Size, string Filtre,
            string TriPar = "Numero")
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);

            CatalogueViewModel viewModel =
                GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);

            ViewBag.NoCategorie = viewModel.Categorie == null ? -1 : viewModel.Categorie.NoCategorie;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count() - 1) / nbItemsParPage + 1;
            ViewBag.Filtre = Filtre;
            ViewBag.Tri = TriPar;
            return View(viewModel);
        }

        public ActionResult ListeProduits(string Vendeur, string Categorie, string Page, string Size, string Filtre,
            string TriPar = "Numero")
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);

            CatalogueViewModel viewModel =
                GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);

            ViewBag.NoCategorie = viewModel.Categorie == null ? -1 : viewModel.Categorie.NoCategorie;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count() - 1) / nbItemsParPage + 1;
            ViewBag.Filtre = Filtre;
            ViewBag.Tri = TriPar;
            return PartialView("Client/Catalogue/_Catalogue", viewModel);
        }

        public ActionResult InformationProduit(int NoProduit)
        {
            PPProduit produit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit).FirstOrDefault();

            return PartialView("Client/Catalogue/_ModalProduit", produit);
        }

        public ActionResult InformationProduitPanier(int NoProduit)
        {
            PPProduit produit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit).FirstOrDefault();

            return PartialView("Client/Panier/_ModalProduit", produit);
        }

        [System.Web.Http.HttpPost]
        public HttpStatusCodeResult AjouterProduitAuPanier(int NoProduit, short Quantite)
        {
            var requeteProduit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit);

            if (!requeteProduit.Any())
                return new HttpStatusCodeResult(HttpStatusCode.Gone);

            if (Quantite > requeteProduit.FirstOrDefault().NombreItems)
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);

            var articlePresent = (from a in context.PPArticlesEnPaniers
                where a.NoClient == NOCLIENT && a.PPProduit.NoProduit == NoProduit
                select a);

            if (articlePresent.Any())
            {
                PPArticlesEnPanier art = articlePresent.FirstOrDefault();
                if (art.NbItems + Quantite > requeteProduit.FirstOrDefault().NombreItems)
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

                art.NbItems += Quantite;
            }
            else
            {
                int noVendeur = (int) requeteProduit.First().NoVendeur;
                DateTime dateCreation = DateTime.Now;
                short nbItems = Quantite;

                long noPanier = (from unPanier in context.PPArticlesEnPaniers
                                    select unPanier.NoPanier).Max() + 1;

                PPArticlesEnPanier article = new PPArticlesEnPanier
                {
                    NoPanier = noPanier,
                    NoClient = NOCLIENT,
                    NoVendeur = noVendeur,
                    DateCreation = dateCreation,
                    NbItems = nbItems,
                    NoProduit = NoProduit
                };
                context.PPArticlesEnPaniers.InsertOnSubmit(article);
            }


            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public bool CheckDisponibiliteArticlesPanier(long NoVendeur)
        {
            Panier panier = GetPanierByVendeurClient((int) NoVendeur);

            if (panier.DepassePoidsMaximum)
                return false;
            foreach (PPArticlesEnPanier article in panier.Articles)
            {
                if (article.NbItems > article.PPProduit.NombreItems)
                    return false;
                if (article.PPProduit.Disponibilité != true)
                    return false;
            }

            return true;
        }

        public ActionResult MonPanier(int id = 0)
        {
            ViewBag.NoClient = NOCLIENT;
            ViewBag.NoVendeur = id;
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);

            try
            {
                ViewBag.NoVendeur = id==0?lstPaniers.FirstOrDefault().Vendeur.No : id;
            }
            catch (Exception e)
            {
                ViewBag.NoVendeur = 0;
            }
            return View(lstPaniers);
        }

        private List<Panier> GetPaniersClient(long NoClient)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NoClient
                orderby articles.DateCreation ascending
                group articles by articles.NoVendeur
                into g
                select g;

            var paniers = query.ToList();

            List<Panier> lstPaniers = new List<Panier>();

            foreach (var pan in paniers)
            {
                Panier panier = new Panier
                {
                    Client = pan.FirstOrDefault().PPClient,
                    Vendeur = pan.FirstOrDefault().PPVendeur,
                    DateCreation = (DateTime) pan.FirstOrDefault().DateCreation,
                    Articles = pan.ToList()
                };
                lstPaniers.Add(panier);
            }

            return lstPaniers;
        }

        private List<PPCommande> GetCommandesClient(long NoClient)
        {
            var query = from commande in context.PPCommandes
                where commande.NoClient == NoClient
                orderby commande.DateCommande ascending
                select commande;

            List<PPCommande> lstCommandes = query.ToList();


            return lstCommandes;
        }

        private Panier GetPanierByVendeurClient(int noVendeur)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == noVendeur
                orderby articles.DateCreation ascending
                select articles;

            if (!query.Any())
                return null;
                
            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };

            return panier;
        }

        private PPVendeur GetVendeurByNo(int no)
        {
            return
                (from v in context.PPVendeurs
                    where v.NoVendeur == no
                    select v).FirstOrDefault();
        }

        public ActionResult Recapitulatif()
        {
            Panier panier = GetPanierByVendeurClient((int) InfoCommande.Panier.Vendeur.No);
            return PartialView("Client/Commande/_RecapitulatifCommande", panier);
        }

        public HtmlString GetPrixLivraison(int noVendeur, decimal poids, decimal prix, int selected)
        {
            var intervallePoids = (from p in context.PPTypesPoids select p).ToList();
            PPTypesPoid codePoids = intervallePoids.FirstOrDefault();
            foreach (var intervalle in intervallePoids)
            {
                if (poids >= intervalle.PoidsMin && poids <= intervalle.PoidsMax)
                {
                    codePoids = intervalle;
                    break;
                }
            }

            PPVendeur vendeur = GetVendeurByNo(noVendeur);

            PPPoidsLivraison livraisons = codePoids.PPPoidsLivraisons.FirstOrDefault(p => p.CodeLivraison == selected);
            decimal? prixLivraison = (prix >= vendeur.LivraisonGratuite && selected == 1)
                ? (decimal?) 0.00
                : livraisons.Tarif;
            InfoCommande.PrixLivraison = prixLivraison;
            InfoCommande.CodeLivraison = selected;

            string str = "Frais de livraison : " + Formatter.Money(prixLivraison, false);
            HtmlString html = new HtmlString(str);
            return html;
        }

        public ActionResult Commande(string id)
        {
            int noVendeur;
            if (!int.TryParse(id, out noVendeur))
                return RedirectToAction("MonPanier", new {No = noVendeur});
            
            Panier panier = GetPanierByVendeurClient(noVendeur);
            if (panier == null) 
                return RedirectToAction("MonPanier", new {No = noVendeur});

            if (!CheckDisponibiliteArticlesPanier(noVendeur))
                return RedirectToAction("MonPanier", new {No = noVendeur});
            
                        
            InfoCommande.Panier = panier;
            return View(panier);
        }

        public ActionResult Information(int noVendeur)
        {
            Panier panier = GetPanierByVendeurClient(noVendeur);
            InfoCommande.Vendeur = panier.Vendeur;

            if (!CheckDisponibiliteArticlesPanier(noVendeur))
                return RedirectToAction("MonPanier", new {No = noVendeur});

            return PartialView("Client/Commande/_Information", panier);
        }

        [System.Web.Http.HttpPost]
        public ActionResult SetInfoClient(InfoClient info)
        {
            info.no = (int) NOCLIENT;
            InfoCommande.InfoClient = (info);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [System.Web.Http.HttpPost]
        public ActionResult SetInfoPaiement(InfoPaiement info)
        {
            if (info == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!new Regex("^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}$").Match(info.NoCarteCredit).Success
                || !new Regex("^[0-9]{2}/[0-9]{2}$").Match(info.DateExpirationCarteCredit).Success
                || !new Regex("^[0-9]{3,4}$").Match(info.NoSecuriteCarteCredit).Success)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
            }

            InfoCommande.InfoPaiement = (info);
            System.Diagnostics.Debug.Write(InfoCommande.InfoClient);

            System.Diagnostics.Debug.Write(InfoCommande.InfoPaiement);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Livraison(int noVendeur)
        {
            Panier panier = GetPanierByVendeurClient(noVendeur);

            //it's bad, but it works
            if (!CheckDisponibiliteArticlesPanier(noVendeur))
                return Content("<script>window.location= '/Client/MonPanier?No="+noVendeur+"'</script>");
            
            return PartialView("Client/Commande/_Livraison",panier);
        }

        public ActionResult Paiement(int noVendeur)
        {
            Panier panier = GetPanierByVendeurClient(noVendeur);
            
            //it's bad, but it works
            if (!CheckDisponibiliteArticlesPanier(noVendeur))
                return Content("<script>window.location= '/Client/MonPanier?No="+noVendeur+"'</script>");
            
            return PartialView("Client/Commande/_Paiement",panier);
        }

        public ActionResult Confirmation(int noVendeur)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == noVendeur
                orderby articles.DateCreation ascending
                select articles;
            
            //it's bad, but it works
            if (!CheckDisponibiliteArticlesPanier(noVendeur))
                return Content("<script>window.location= '/Client/MonPanier?No="+noVendeur+"'</script>");

            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };
            return PartialView("Client/Commande/_Confirmation", panier);
        }

        public ActionResult DetailPanier(int noVendeur)
        {
            if (noVendeur == 0)
            {
                Panier panierFirst = GetPaniersClient(NOCLIENT).FirstOrDefault();
                return PartialView("Client/Panier/_DetailPanier", panierFirst);
            }

            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == noVendeur
                orderby articles.DateCreation ascending
                select articles;

            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };
            return PartialView("Client/Panier/_DetailPanier", panier);
        }

        public ActionResult SupprimerArticle(int NoProduit, int NoVendeur)
        {
            var articleSuprimer = (from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == NoVendeur
                      && articles.PPProduit.NoProduit == NoProduit
                select articles).FirstOrDefault();

            context.PPArticlesEnPaniers.DeleteOnSubmit(articleSuprimer);
            context.SubmitChanges();

            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == NoVendeur
                orderby articles.DateCreation ascending
                select articles;

            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };

            return PartialView("Client/Panier/_DetailPanier", panier);
        }

        public ActionResult SupprimerPanier(int NoVendeur)
        {
            var articlesSuprimer = (from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == NoVendeur
                select articles).ToList();

            foreach (var article in articlesSuprimer)
            {
                context.PPArticlesEnPaniers.DeleteOnSubmit(article);
            }

            context.SubmitChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult ModifierQuantite(int NoProduit, int NoVendeur, bool aAugmenter)
        {
            var articleModifer = (from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == NoVendeur
                      && articles.PPProduit.NoProduit == NoProduit
                select articles).FirstOrDefault();

            //augmenter
            if (aAugmenter)
            {
                if (articleModifer.PPProduit.NombreItems > articleModifer.NbItems)
                {
                    articleModifer.NbItems += 1;
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
            }
            //diminuer
            else if (articleModifer.NbItems > 1)
            {
                articleModifer.NbItems -= 1;
            }

            context.SubmitChanges();

            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == NoVendeur
                orderby articles.DateCreation ascending
                select articles;

            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };

            return PartialView("Client/Panier/_DetailPanier", panier);
        }

        private int GetNextNoCommande()
        {
            var requete = (context.PPCommandes.OrderByDescending(c => c.NoCommande));
            if (requete.Any())
            {
                return (int) (requete.FirstOrDefault().NoCommande + 1);
            }

            return 1;
        }

        private long GetNextNoHistoriquePaiement()
        {
            var requete = (context.PPHistoriquePaiements.OrderByDescending(c => c.NoHistorique));
            if (requete.Any())
            {
                return (long) (requete.FirstOrDefault().NoHistorique + 1);
            }

            return 1;
        }

        private long GetNextNoDetailsCommande()
        {
            var requete = (context.PPDetailsCommandes.OrderByDescending(c => c.NoDetailCommandes));
            if (requete.Any())
            {
                return (long) (requete.FirstOrDefault().NoDetailCommandes + 1);
            }

            return 1;
        }

        private void UpdateInfoClient()
        {
            PPClient clientAModifier = (from c in context.PPClients
                where c.NoClient == NOCLIENT
                select c).FirstOrDefault();

            if (clientAModifier != null)
            {
                clientAModifier.Nom = InfoCommande.InfoClient.nom;
                clientAModifier.Prenom = InfoCommande.InfoClient.prenom;
                clientAModifier.Tel1 = InfoCommande.InfoClient.telephone;
                clientAModifier.Tel2 = InfoCommande.InfoClient.cellulaire;
                clientAModifier.Rue = InfoCommande.InfoClient.rue;
                clientAModifier.Ville = InfoCommande.InfoClient.ville;
                clientAModifier.Province = InfoCommande.InfoClient.province;
                clientAModifier.Pays = "Canada";
                clientAModifier.CodePostal = InfoCommande.InfoClient.codePostal;
            }
        }

        private PPCommande CreateCommande(DateTime DateAutorisation, string NoAutorisation)
        {
            PPCommande commande = new PPCommande
            {
                NoCommande = GetNextNoCommande(),
                NoClient = NOCLIENT,
                NoVendeur = InfoCommande.Vendeur.No,
                DateCommande = DateAutorisation,
                CoutLivraison = InfoCommande.PrixLivraison,
                TypeLivraison = (short) InfoCommande.CodeLivraison,
                MontantTotAvantTaxes = InfoCommande.Panier.getPrixTotal() + InfoCommande.PrixLivraison,
                TPS = InfoCommande.Panier.GetTPS((decimal) InfoCommande.PrixLivraison),
                TVQ = InfoCommande.Panier.GetTVQ((decimal) InfoCommande.PrixLivraison),
                PoidsTotal = InfoCommande.Panier.GetPoidsTotal(),
                Statut = 'T',
                NoAutorisation = NoAutorisation
            };
            return commande;
        }

        private PPHistoriquePaiement CreatePaiement(PPCommande commande, string NoAutorisation, string FraisMarchand)
        {
            PPHistoriquePaiement paiement = new PPHistoriquePaiement
            {
                NoHistorique = GetNextNoHistoriquePaiement(),
                MontantVenteAvantLivraison = InfoCommande.Panier.getPrixTotal(),
                FraisLivraison = commande.CoutLivraison,
                NoVendeur = commande.NoVendeur,
                NoClient = commande.NoClient,
                NoCommande = commande.NoCommande,
                DateVente = commande.DateCommande,
                NoAutorisation = NoAutorisation,
                FraisLesi = Convert.ToDecimal(FraisMarchand, new CultureInfo("en-CA")),
                Redevance = InfoCommande.Vendeur.Pourcentage * InfoCommande.Panier.getPrixTotal(),
                FraisTPS = commande.TPS,
                FraisTVQ = commande.TVQ
            };
            return paiement;
        }

        public ActionResult ConfirmationPaiement(string NoAutorisation, DateTime DateAutorisation, string FraisMarchand,
            string InfoSuppl)
        {
            if (!CheckDisponibiliteArticlesPanier(InfoCommande.Vendeur.No))
                return View("ErreurCommande", 9999);

            Panier panVerif = GetPanierByVendeurClient((int) InfoCommande.Vendeur.No);
            if(panVerif.Articles.Count != InfoCommande.Panier.Articles.Count)
                return View("ErreurCommande", 9999);
            foreach (PPArticlesEnPanier article in panVerif.Articles)
            {
                PPArticlesEnPanier artVerif = InfoCommande.Panier.Articles.First(a => a.NoProduit == article.NoProduit);
                
                if(artVerif == null)
                    return View("ErreurCommande", 9999);
                if(artVerif.NbItems != article.NbItems)
                    return View("ErreurCommande", 9999);
            }
            
            {
                if(GetPanierByVendeurClient((int) InfoCommande.Vendeur.No) == InfoCommande.Panier)
                    return View("ErreurCommande", 9999);
                
            }
            if (NoAutorisation == "9999" || NoAutorisation == "1" || NoAutorisation == "2" || NoAutorisation == "3")
            {
                int intNoAutorisation;
                int.TryParse(NoAutorisation, out intNoAutorisation);
                return View("ErreurCommande", intNoAutorisation);
            }

            try
            {
                UpdateInfoClient();

                PPCommande commande = CreateCommande(DateAutorisation, NoAutorisation);
                context.PPCommandes.InsertOnSubmit(commande);

                PPHistoriquePaiement paiement = CreatePaiement(commande, NoAutorisation, FraisMarchand);
                context.PPHistoriquePaiements.InsertOnSubmit(paiement);

                long noDetail = GetNextNoDetailsCommande();
                foreach (PPArticlesEnPanier article in GetPanierByVendeurClient((int)InfoCommande.Panier.Vendeur.No).Articles)
                {
                    PPArticlesEnPanier articleASupprimer =
                        (from c in context.PPArticlesEnPaniers
                            where c.NoPanier == article.NoPanier
                            select c).FirstOrDefault();

                    context.PPArticlesEnPaniers.DeleteOnSubmit(articleASupprimer);

                    PPDetailsCommande detailsCommande = new PPDetailsCommande
                    {
                        NoDetailCommandes = noDetail++,
                        NoCommande = commande.NoCommande,
                        NoProduit = article.NoProduit,
                        PrixVente = article.PPProduit.GetPrixCourant(),
                        Quantité = article.NbItems
                    };

                    PPProduit produitAModifier = (from p in context.PPProduits
                        where p.NoProduit == article.PPProduit.NoProduit
                        select p).First();

                    produitAModifier.NombreItems -= article.NbItems;
                    if (produitAModifier.NombreItems < 0)
                        return View("ErreurCommande", 4);

                    context.PPDetailsCommandes.InsertOnSubmit(detailsCommande);
                }

                context.SubmitChanges();

                ComposerMessage(commande);
                return RedirectToAction("Recu", "Client",
                    new RouteValueDictionary(new {noCommande = commande.NoCommande}));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View("ErreurCommande", 101);
            }
        }

        private void ComposerMessage(PPCommande commande)
        {
            var maxId = (from m in context.PPMessages
                select m.NoMsg).ToList().DefaultIfEmpty().Max();
            var noMessage = maxId + 1;
            var descMsg = PartialView("Vendeur/_RecuCommande",commande).RenderToString();
            
            
            context.PPMessages.InsertOnSubmit(
                new PPMessage
                {
                    NoMsg = noMessage,
                    objet = "Nouvelle commande",
                    Lieu = 5,
                    dateEnvoi = DateTime.Now,
                    NoExpediteur = 101,
                    DescMsg = descMsg
                });
            
            context.PPDestinataires.InsertOnSubmit(
                new PPDestinataire
                {
                    Lieu = 1,
                    EtatLu = 0,
                    NoMsg = noMessage,
                    NoDestinataire = (int) NOCLIENT
                });

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private void genererPDF(PPCommande commande)
        {
            string view;
            PartialViewResult vr = PartialView("Vendeur/_RecuCommande", commande);

            using (var sw = new StringWriter())
            {
                vr.View = ViewEngines.Engines
                  .FindPartialView(ControllerContext, vr.ViewName).View;

                var vc = new ViewContext(
                  ControllerContext, vr.View, vr.ViewData, vr.TempData, sw);
                vr.View.Render(vc, sw);

                view = sw.GetStringBuilder().ToString();
            }

            IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
            var PDF = Renderer.RenderHtmlAsPdf(view);
            string path = AppDomain.CurrentDomain.BaseDirectory + "Recus/" + commande.NoCommande + ".pdf";
            PDF.TrySaveAs(path);
        }

        [System.Web.Http.HttpPost]
        public ActionResult EnvoyerEvaluation(int cote, string commentaire, int noProduit)
        {
            try
            {
                if (cote > 5 || cote < 0)
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);

                PPEvaluation evaluation = new PPEvaluation
                {
                    Cote_ = cote,
                    Commentaire_ = commentaire,
                    NoClient = NOCLIENT,
                    NoProduit = noProduit,
                    DateCreation_ = DateTime.Now,
                    DateMAJ_ = DateTime.Now
                };
                context.PPEvaluations.InsertOnSubmit(evaluation);
                context.SubmitChanges();

                return PartialView("Client/Catalogue/_EtoilesRating", evaluation.PPProduit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                ;
            }
        }

        [System.Web.Http.HttpPost]
        public ActionResult ModifierEvaluation(int cote, string commentaire, int noProduit)
        {
            try
            {
                if (cote > 5 || cote < 0)
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);

                PPEvaluation evaluation = (from e in context.PPEvaluations
                    where e.NoProduit == noProduit && e.NoClient == NOCLIENT
                    select e).First();

                evaluation.Cote_ = cote;
                evaluation.Commentaire_ = commentaire;
                evaluation.DateMAJ_ = DateTime.Now;

                context.SubmitChanges();

                return PartialView("Client/Catalogue/_EtoilesRating", evaluation.PPProduit);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
                ;
            }
        }

        public ActionResult Recu(int noCommande)
        {
            var commande = (from c in context.PPCommandes
                where c.NoCommande == noCommande && c.NoClient == NOCLIENT
                select c);

            if (!commande.Any())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            return View("ResultatCommande", commande.First());
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Profil(string status="")
        {
            ViewBag.status = status;
            var objClient = (from unClient in context.PPClients
                where unClient.NoClient == NOCLIENT
                select unClient).FirstOrDefault();

            ModiProfilClient modiProfilClient = new ModiProfilClient
            {
                Nom = objClient.Nom,
                Prenom = objClient.Prenom,
                Province = objClient.Province,
                Rue = objClient.Rue,
                Tel1 = objClient.Tel1,
                Tel2 = objClient.Tel2,
                CodePostal = objClient.CodePostal,
                Pays = objClient.Pays,
                Ville = objClient.Ville
            };
            return View(modiProfilClient);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Profil(ModiProfilClient objClient)
        {
            if (ModelState.IsValid)
            {
                var ClientData = (from cli in context.PPClients
                    where cli.NoClient == NOCLIENT
                    select cli).First();


                if (objClient.Nom != "") ClientData.Nom = objClient.Nom; else ClientData.Nom ="";
                if (objClient.Prenom != "") ClientData.Prenom = objClient.Prenom; else ClientData.Prenom = "";
                if (objClient.Rue != "") ClientData.Rue = objClient.Rue; else ClientData.Rue = "";
                if (objClient.Ville != "") ClientData.Ville = objClient.Ville; else ClientData.Ville = "";
                if (objClient.Province != "") ClientData.Province = objClient.Province; else ClientData.Province = "";
                if (objClient.CodePostal != "") ClientData.CodePostal = objClient.CodePostal; else ClientData.CodePostal = "";
                if (objClient.Pays != "") ClientData.Pays = objClient.Pays; else ClientData.Pays = "";
                if (objClient.Tel1 != "") ClientData.Tel1 = objClient.Tel1; else ClientData.Tel1 = "";
                if (objClient.Tel2 != "") ClientData.Tel2 = objClient.Tel2; else ClientData.Tel2 = "";
                try
                {
                    context.SubmitChanges();

                    return RedirectToAction("Profil", new {Status = "ModificationReussite"});
                }
                catch (Exception e)
                {
                }
            }

            return View();
        }
        public ActionResult PdfCommande(int id)
        {
            var user = SessionUtilisateur.UtilisateurCourant;

            var query = from commandes in context.PPCommandes
                where commandes.NoCommande == id
                select commandes;

            var commande = query.FirstOrDefault();

            if (user is PPVendeur)
            {
                PPVendeur vendeur = (PPVendeur)user;
                if(commande.PPVendeur.NoVendeur != vendeur.NoVendeur)
                    return new HttpStatusCodeResult(400, "Id commande invalide");
            }
            else if (user is PPClient)
            {
                PPClient client = (PPClient)user;
                if(commande.PPClient.NoClient != client.NoClient)
                    return new HttpStatusCodeResult(400, "Id commande invalide");
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "Recus/" + id + ".pdf";

            if (!System.IO.File.Exists(path))
                genererPDF(commande);

            return File(path, "application/pdf");
            
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult modificationMDP()
        {
            ModificationMDP modificationMdp = new ModificationMDP
            {
                ancienMDP = "",
                motDePass = "",
                confirmationMDP = ""
            };
            return View(modificationMdp);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult modificationMDP(ModificationMDP modificationMdp)
        {
            var clientCourrant = (from unClient in context.PPClients
                where unClient.NoClient == NOCLIENT
                select unClient).First();

            bool ancienMDPValide = clientCourrant.MotDePasse == modificationMdp.ancienMDP;
            if (ModelState.IsValid)
            {
                if (!ancienMDPValide)
                {
                    ModelState.AddModelError(string.Empty, "L'ancien mot de passe est invalide.");
                    return View(modificationMdp);
                }

                try
                {
                    clientCourrant.MotDePasse = modificationMdp.motDePass;
                    context.SubmitChanges();

                    return RedirectToAction("Index",new {status="ModificationReussite"});
                }
                catch (Exception e)
                {
                }
            }

            return View(modificationMdp);
        }

        public ActionResult SuccessInscription()
        {
            return View();
        }
        [System.Web.Mvc.HttpGet]
        public ActionResult InscriptionVendeur()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult InscriptionVendeur(FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                var tousLesVendeurs = from unVendeur in context.PPVendeurs select unVendeur.NoVendeur;

                int maxNoVendeur = Convert.ToInt32(tousLesVendeurs.Max()) + 1;

                var VerificationAdresseCourriel = from unVendeur in context.PPVendeurs
                    where unVendeur.AdresseEmail == formCollection["AdresseEmail"]
                    select unVendeur;

                PPVendeur nouveauVendeur = new PPVendeur();

                if (VerificationAdresseCourriel.Count() > 0)
                    ModelState.AddModelError("AdresseEmail",
                        "Cette adresse courriel est déjà utilisée, veuillez réessayer un nouveau!");
                
                nouveauVendeur.NoVendeur = maxNoVendeur;
                nouveauVendeur.NomAffaires = formCollection["Vendeur.NomAffaires"];
                nouveauVendeur.Nom = formCollection["Vendeur.Nom"];
                nouveauVendeur.Prenom = formCollection["Vendeur.Prenom"];
                nouveauVendeur.Rue = formCollection["Vendeur.Rue"];
                nouveauVendeur.Ville = formCollection["Vendeur.Ville"];
                nouveauVendeur.Province = formCollection["Vendeur.Province"];
                nouveauVendeur.CodePostal = formCollection["Vendeur.CodePostal"];
                nouveauVendeur.Pays = formCollection["Vendeur.Pays"];
                nouveauVendeur.Tel1 = formCollection["Vendeur.Tel1"];
                nouveauVendeur.Tel2 = formCollection["Vendeur.Tel2"];
                nouveauVendeur.AdresseEmail = formCollection["AdresseEmail"];
                nouveauVendeur.PoidsMaxLivraison = Convert.ToInt32(formCollection["Vendeur.PoidsMaxLivraison"]);
                nouveauVendeur.LivraisonGratuite = Convert.ToDecimal(formCollection["Vendeur.LivraisonGratuite"]);
                nouveauVendeur.MotDePasse = formCollection["MotDePasse"];
                nouveauVendeur.Configuration =
                    "color:#000000; background-color:#FFFFFF; font-family:Comic Sans ms;";
                nouveauVendeur.Taxes = formCollection["Taxes"] == "on" ? true : false;
                nouveauVendeur.DateCreation = DateTime.Now;

                nouveauVendeur.Statut = 0;
                try
                {
                    context.PPVendeurs.InsertOnSubmit(nouveauVendeur);
                    context.SubmitChanges();
                    return RedirectToAction("SuccessInscription");
                }
                catch (Exception e)
                {
                    return View();
                }
                
            }

            return View();
        }
    }
}
