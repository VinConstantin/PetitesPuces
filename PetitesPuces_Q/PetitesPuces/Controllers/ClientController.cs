using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.Utilities;
using PetitesPuces.ViewModels.Vendeur;
using Rotativa;

namespace PetitesPuces.Controllers
{
    #if !DEBUG
        [Securise(RolesUtil.CLIENT)]
    #endif
    public class ClientController : Controller
    {
        private long NOCLIENT = SessionUtilisateur.UtilisateurCourant.No;
        private const int DEFAULTITEMPARPAGE = 8;
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);
            List<PPCommande> lstCommandes = GetCommandesClient(NOCLIENT);
            
            AccueilViewModel viewModel = new AccueilViewModel
            {
                Paniers = lstPaniers,
                Commandes = lstCommandes
            };
            return View(viewModel);
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
                listeProduits = (from p in context.PPProduits select p)
                    .Where(p => categorie == null || p.PPCategory == categorie);
            }
            else
            {
                
                var requete = (from unVendeur in context.PPVendeurs 
                    where unVendeur.NoVendeur == NoVendeur
                    select unVendeur);
                vendeur = requete.FirstOrDefault();
                //creer la liste de produits
                listeProduits = vendeur.PPProduits
                    .Where(p => categorie == null || p.PPCategory == categorie);
            }

            if(!String.IsNullOrEmpty(Filtre)) 
                listeProduits = listeProduits.Where(p => p.Nom.ToLower().Contains(Filtre.ToLower()));

            switch (tri)
            {
                case Tri.Nom :
                    listeProduits = listeProduits.OrderBy(p => p.Nom);
                    break;
                case Tri.Categorie :
                    listeProduits = listeProduits.OrderBy(p => p.PPCategory.Description);
                    break;
                case Tri.Date :
                    listeProduits = listeProduits.OrderBy(p => p.DateCreation);
                    break;
                case Tri.Numero :
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
        public ActionResult Catalogue(string Vendeur, string Categorie, string Page, string Size, string Filtre, string TriPar="Numero")
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);
            
            CatalogueViewModel viewModel = GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);
            
            ViewBag.NoCategorie = viewModel.Categorie==null?-1 : viewModel.Categorie.NoCategorie;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count()-1)/nbItemsParPage+1;
            ViewBag.Filtre = Filtre;
            ViewBag.Tri = TriPar;
            return View(viewModel);
        }
        public ActionResult ListeProduits(string Vendeur, string Categorie, string Page, string Size, string Filtre, string TriPar="Numero")
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);
            
            CatalogueViewModel viewModel = GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);
            
            ViewBag.NoCategorie = viewModel.Categorie==null?-1 : viewModel.Categorie.NoCategorie;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count()-1)/nbItemsParPage+1;
            ViewBag.Filtre = Filtre;
            ViewBag.Tri = TriPar;
            return PartialView("Client/_Catalogue",viewModel);
        }

        public ActionResult InformationProduit(int NoProduit)
        {
            PPProduit produit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit).FirstOrDefault();

            return PartialView("Client/ModalProduit", produit);

        }
        [System.Web.Mvc.HttpPost]
        public HttpStatusCode AjouterProduitAuPanier(int NoProduit, short Quantite)
        {
            var requeteProduit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit);

            if (!requeteProduit.Any())
                return HttpStatusCode.Gone;

            if (Quantite > requeteProduit.FirstOrDefault().NombreItems)
                return HttpStatusCode.Conflict;

            var articlePresent = (from a in context.PPArticlesEnPaniers
                where a.NoClient == NOCLIENT && a.PPProduit.NoProduit == NoProduit
                select a);
            
            if (articlePresent.Any())
            {
                PPArticlesEnPanier art = articlePresent.FirstOrDefault();
                if (art.NbItems + Quantite > requeteProduit.FirstOrDefault().NombreItems)
                    return HttpStatusCode.Conflict;

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
                return HttpStatusCode.InternalServerError;
            }         
            return HttpStatusCode.OK;
        }

        public ActionResult CheckDisponibiliteArticlesPanier(int NoVendeur)
        {
            Panier panier = GetPanierByVendeurClient(NoVendeur);

            foreach (PPArticlesEnPanier article in panier.Articles)
            {
                if (article.NbItems > article.PPProduit.NombreItems)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        } 
        public ActionResult MonPanier(string No)
        {
            ViewBag.NoClient = NOCLIENT;
            ViewBag.NoVendeur = No;
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);
            //List<Panier> lstPaniers = new List<Panier>();
            return View(lstPaniers);
        }
        private List<Panier> GetPaniersClient(long NoClient)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NoClient
                orderby articles.DateCreation ascending
                group articles by articles.NoVendeur into g
                select g;

            var paniers = query.ToList();

            List<Panier> lstPaniers = new List<Panier>();

            foreach (var pan in paniers)
            {
                Panier panier = new Panier
                {
                    Client = pan.FirstOrDefault().PPClient,
                    Vendeur = pan.FirstOrDefault().PPVendeur,
                    DateCreation = (DateTime)pan.FirstOrDefault().DateCreation,
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

        public HtmlString GetPrixLivraison(int noVendeur, decimal poids, decimal prix, int selected)
        {
            var intervallePoids = (from p in context.PPTypesPoids select p).ToList();
            PPTypesPoid codePoids = intervallePoids.FirstOrDefault();
            foreach (var intervalle in intervallePoids)
            {
                if(poids>=intervalle.PoidsMin && poids<=intervalle.PoidsMax)
                {
                    codePoids = intervalle;
                    break;
                }
            }
            PPVendeur vendeur = GetVendeurByNo(noVendeur);

            PPPoidsLivraison livraisons = codePoids.PPPoidsLivraisons.FirstOrDefault(p => p.CodeLivraison == selected);
            decimal? prixLivraison = (prix >= vendeur.LivraisonGratuite && selected==1)?
                (decimal?)0.00:livraisons.Tarif;
            InfoCommande.PrixLivraison = prixLivraison;
            InfoCommande.CodeLivraison = selected;
            
            string str = "Frais de livraison : " + Formatter.Money( prixLivraison, false);
            HtmlString html = new HtmlString(str);
            return html;
        }
        public ActionResult Commande(string Etape, int noVendeur)
        {
            ViewBag.Etape = Etape;

            Panier panier = GetPanierByVendeurClient(noVendeur);
            InfoCommande.Panier = panier;
            return View(panier);
        }

        public ActionResult Information(int noVendeur)
        {
            Panier panier = GetPanierByVendeurClient(noVendeur);
            InfoCommande.Vendeur = panier.Vendeur;
            
            return PartialView("Client/Commande/_Information", panier);
        }     
        [System.Web.Http.HttpPost]
        public ActionResult SetInfoClient(InfoClient info)
        {
            info.no = (int)NOCLIENT;
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
                ||!new Regex("^[0-9]{2}/[0-9]{2}$").Match(info.DateExpirationCarteCredit).Success
                ||!new Regex("^[0-9]{3,4}$").Match(info.NoSecuriteCarteCredit).Success)
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
            
            return PartialView("Client/Commande/_Livraison",panier);
        }
        public ActionResult Paiement(int noVendeur)
        {
            Panier panier = GetPanierByVendeurClient(noVendeur);
            return PartialView("Client/Commande/_Paiement",panier);
        }
        public ActionResult Confirmation(int noVendeur)
        {
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
            return PartialView("Client/Commande/_Confirmation",panier);
        }

        public ActionResult DetailPanier(int noVendeur)
        {
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
            return PartialView("Client/_DetailPanier",panier);
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
            
            return PartialView("Client/_DetailPanier",panier);
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
            else if(articleModifer.NbItems > 1)
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
            
            return PartialView("Client/_DetailPanier",panier);
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

        private PPCommande CreateCommande(DateTime DateAutorisation,string NoAutorisation)
        {
            PPCommande commande = new PPCommande
            {
                NoCommande = GetNextNoCommande(),
                NoClient = NOCLIENT,
                NoVendeur = InfoCommande.Vendeur.No,
                DateCommande = DateAutorisation,
                CoutLivraison = InfoCommande.PrixLivraison,
                TypeLivraison = (short)InfoCommande.CodeLivraison,
                MontantTotAvantTaxes = InfoCommande.Panier.getPrixTotal() + InfoCommande.PrixLivraison,
                TPS = (decimal)((double)(InfoCommande.Panier.getPrixTotal() + InfoCommande.PrixLivraison)*0.05),
                TVQ = (decimal)((double)(InfoCommande.Panier.getPrixTotal() + InfoCommande.PrixLivraison)*0.0975),
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
                NoVendeur = commande.NoVendeur,
                NoClient = commande.NoClient,
                NoCommande = commande.NoCommande,
                DateVente = commande.DateCommande,
                NoAutorisation = NoAutorisation,
                FraisLesi = Convert.ToDecimal(FraisMarchand,new CultureInfo("en-CA")),
                Redevance = InfoCommande.Vendeur.Pourcentage*InfoCommande.Panier.getPrixTotal(),
                FraisTPS = commande.TPS,
                FraisTVQ = commande.TVQ
            };
            return paiement;
        }

        public ActionResult ConfirmationPaiement(string NoAutorisation, DateTime DateAutorisation, string FraisMarchand,
            string InfoSuppl)
        {
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
                foreach (PPArticlesEnPanier article in InfoCommande.Panier.Articles)
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
                    if(produitAModifier.NombreItems < 0)
                        return View("ErreurCommande",4);
                    
                    context.PPDetailsCommandes.InsertOnSubmit(detailsCommande);
                }  

                context.SubmitChanges();
                return RedirectToAction("Recapitulatif", "Client",
                    new RouteValueDictionary(new { noCommande = commande.NoCommande}));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View("ErreurCommande",101);
            }
        }
        
        public ActionResult Recapitulatif(int noCommande)
        {
            var commande = (from c in context.PPCommandes 
                where c.NoCommande == noCommande && c.NoClient==NOCLIENT
                select c);
            
            if(!commande.Any())
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            
            return View("ResultatCommande", commande.First());
        }
        public ActionResult Profil()
        {

            return View();
        }
        public ActionResult modificationMDP(FormCollection formCollection)
        {
          /*  PPClient unClient=new PPClient();

            var motDePasseCourrant=formCollection[""]
            var nouveauMotdePasse;
            */
            return View();
        }

    }
}
