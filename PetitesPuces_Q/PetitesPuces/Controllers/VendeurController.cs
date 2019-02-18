using PetitesPuces.Models;
using PetitesPuces.ViewModels.Vendeur;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Securite;
using PetitesPuces.Utilities;

using IronPdf;
using System.Net;

namespace PetitesPuces.Controllers
{
#if !DEBUG
        [Securise(RolesUtil.VEND)]
#endif
    public class VendeurController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        int NoVendeur = 11;
        // GET: Vendeur
        public ActionResult Index()
        {
            List<PPCommande> commandes = GetCommandesVendeurs(NoVendeur);

            commandes = commandes.Where(c => c.Statut == 'T').ToList();

            var viewModel = new AccueilVendeurViewModel
            {
                Commandes = commandes,
                Paniers = GetPaniersVendeurs(NoVendeur),
                NbVisites = GetNbVisiteurs(NoVendeur)
            };
            return View(viewModel);
        }

        public ActionResult GestionCommandes()
        {
            return View(GetCommandesVendeurs(NoVendeur));
        }

        public ActionResult GestionPaniers()
        {
            List<Panier> paniers = GetPaniersVendeurs(NoVendeur);

            paniers = paniers.Where(p => DateTime.Today.AddMonths(-6) >= p.DateCreation).ToList();

            return View(paniers);
        }

        public ActionResult GestionCatalogue()
        {
            List<PPProduit> Produits = GetProduitsVendeurs(NoVendeur);
            long NoProduit = Produits.Count() != 0 ? Produits.LastOrDefault().NoProduit + 1 : long.Parse(NoVendeur + "00001");

            var viewModel = new GestionCatalogueViewModel
            {
                NoProduit = NoProduit,
                Produits = Produits,
                Categories = GetCategories()
            };

            return View(viewModel);
        }

        public ActionResult InfoClient(int id)
        {
            var client = from clients in context.PPClients
                         where clients.NoClient == id
                         select clients;

            return PartialView("Vendeur/ModalInfoClient", client.FirstOrDefault());
        }

        [Securise(RolesUtil.CLIENT, RolesUtil.VEND)]
        public ActionResult InfoCommande(int id)
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

        public ActionResult InfoPanier(int id)
        {
            List<Panier> paniers = GetPaniersVendeurs(NoVendeur);
            Panier panier = paniers.Single(p => p.Client.NoClient == id);

            return PartialView("Vendeur/ModalInfoPanier", panier);
        }

        public ActionResult Profil()
        {
            return View();
        }

        public ActionResult VisualiserPaniers(int NbMois)
        {
            List<Panier> paniers = GetPaniersVendeurs(NoVendeur);
            if (NbMois != 99)
            {
                paniers = paniers.Where(p => DateTime.Today.AddMonths(-NbMois) <= p.DateCreation).ToList();
            }
            return PartialView("Vendeur/_PaniersMois", paniers);
        }

        public ActionResult ConfirmationLivraison(int NoCommande)
        {
            return PartialView("Vendeur/_ConfirmationLivraison", NoCommande);
        }

        public ActionResult ConfirmationPanier(int NoClient)
        {
            return PartialView("Vendeur/_ConfirmationPanier", NoClient);
        }

        public ActionResult GestionProduit(int NoProduit)
        {
            var query = from produit in context.PPProduits
                        where produit.NoProduit == NoProduit
                        select produit;
            return PartialView("Vendeur/_GestionProduit", query.FirstOrDefault());
        }

        public ActionResult ModalModifierProduit(int NoProduit) //TODO
        {
            var query = from produit in context.PPProduits
                        where produit.NoProduit == NoProduit
                        select produit;
            var viewModel = new ModifierProduitViewModel
            {
                Categories = GetCategories(),
                Produit = query.FirstOrDefault()
            };
            return PartialView("Vendeur/ModalModifierProduit", viewModel);
        }

        public ActionResult ModalSupprimerProduit(long NoProduit) //TODO
        {
            var produit = (from produits in context.PPProduits
                           where produits.NoProduit == NoProduit
                           select produits).FirstOrDefault();

            string strBody = "<p>Êtes-vous sûr de vouloir supprimer ce produit?</p>";

            if (produit.PPArticlesEnPaniers.Count() != 0)
            {
                strBody += "<p>Ce produit est présent dans un ou plusieurs paniers</p>";
            }

            if (produit.PPDetailsCommandes.Count() != 0)
            {
                strBody += "<p>Ce produit est présent dans des commandes donc il sera désactivé</p>";
            }

            var viewModel = new SupprimerProduitViewModel
            {
                NoProduit = NoProduit,
                StrBody = StringExtension.ToHtml(strBody)
            };

            return PartialView("Vendeur/ModalSupprimerProduit", viewModel);
        }

        public ActionResult Evaluations(int NoProduit)
        {
            List<PPEvaluation> evaluations = (from e in context.PPEvaluations
                                              where e.NoProduit == NoProduit && e.PPProduit.NoVendeur == NoVendeur
                                              select e).ToList();

            PPProduit produit = (from p in context.PPProduits
                                 where p.NoProduit == NoProduit
                                 select p).FirstOrDefault();

            if (produit == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(new Tuple<List<PPEvaluation>, PPProduit>(evaluations, produit));
        }

        [HttpPost]
        public long ModifierProduit() //TODO
        {
            NameValueCollection nvc = Request.Form;

            var produit = (from produits in context.PPProduits
                           where produits.NoProduit == long.Parse(nvc["NoProduit"])
                           select produits).FirstOrDefault();

            produit.NoCategorie = int.Parse(nvc["NoCategorie"]);
            produit.NombreItems = short.Parse(nvc["NombreItems"]);
            produit.Nom = nvc["Nom"];
            produit.PrixDemande = decimal.Parse(nvc["PrixDemande"]);
            produit.Poids = decimal.Parse(nvc["Poids"]);
            produit.Description = nvc["Description"];
            produit.Disponibilité = nvc["Disponibilite"] == "on" ? true : false;
            produit.DateMAJ = DateTime.Today;

            if (decimal.TryParse(nvc["PrixVente"], out decimal prixVente) &&
                DateTime.TryParse(nvc["DateVente"], out DateTime date))
            {
                produit.PrixVente = prixVente;
                produit.DateVente = date;
            }

            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase hpfb = Request.Files.Get(0);

                if (hpfb.FileName != "")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/images/produits/";
                    string filename = produit.NoProduit.ToString() + Path.GetExtension(hpfb.FileName);
                    produit.Photo = filename;
                    hpfb.SaveAs(Path.Combine(path, filename));
                }
            }

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return produit.NoProduit;
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

        public void SupprimerProduit(int NoProduit) //TODO
        {
            var produit = (from produits in context.PPProduits
                           where produits.NoProduit == NoProduit
                           select produits).FirstOrDefault();

            produit.NombreItems = 0;

            if (produit.PPArticlesEnPaniers.Count() != 0)
            {
                context.PPArticlesEnPaniers.DeleteAllOnSubmit(produit.PPArticlesEnPaniers);
            }

            if (produit.PPDetailsCommandes.Count() != 0)
            {
                produit.Disponibilité = null;
            }
            else
            {
                context.PPProduits.DeleteOnSubmit(produit);
            }

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Livraison(int NoCommande)
        {
            var query = from commandes in context.PPCommandes
                        where commandes.NoCommande == NoCommande
                        select commandes;

            query.FirstOrDefault().Statut = 'L';
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SuppressionPanier(int NoClient)
        {
            var query = from articles in context.PPArticlesEnPaniers
                        where articles.NoVendeur == NoVendeur && articles.NoClient == NoClient
                        select articles;

            context.PPArticlesEnPaniers.DeleteAllOnSubmit(query.ToList());

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [HttpPost]
        public string AjouterProduit()
        {
            NameValueCollection nvc = Request.Form;
            PPProduit produit = new PPProduit
            {
                NoProduit = long.Parse(nvc["NoProduit"]),
                DateCreation = DateTime.Parse(nvc["DateCreation"]),
                NoCategorie = int.Parse(nvc["NoCategorie"]),
                NombreItems = short.Parse(nvc["NombreItems"]),
                Nom = nvc["Nom"],
                PrixDemande = decimal.Parse(nvc["PrixDemande"]),
                Poids = decimal.Parse(nvc["Poids"]),
                Description = nvc["Description"],
                Disponibilité = nvc["Disponibilite"] == "on" ? true : false,
                NoVendeur = NoVendeur,
                DateMAJ = DateTime.Parse(nvc["DateCreation"])
            };

            if (decimal.TryParse(nvc["PrixVente"], out decimal prixVente) &&
                DateTime.TryParse(nvc["DateVente"], out DateTime date))
            {
                produit.PrixVente = prixVente;
                produit.DateVente = date;
            }

            if (Request.Files.Count != 0)
            {
                HttpPostedFileBase hpfb = Request.Files.Get(0);

                if (hpfb.FileName != "")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "/images/produits/";
                    string filename = produit.NoProduit.ToString() + Path.GetExtension(hpfb.FileName);
                    produit.Photo = filename;
                    hpfb.SaveAs(Path.Combine(path, filename));
                }
            }

            context.PPProduits.InsertOnSubmit(produit);
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "success";
        }

        private List<PPCategory> GetCategories()
        {
            var query = from categories in context.PPCategories
                        select categories;

            return query.ToList();
        }

        private List<PPProduit> GetProduitsVendeurs(int NoVendeur)
        {
            var query = from produits in context.PPProduits
                        where produits.NoVendeur == NoVendeur
                        orderby produits.NoProduit ascending
                        select produits;

            return query.ToList();
        }

        private List<PPCommande> GetCommandesVendeurs(int NoVendeur)
        {
            var query = from commandes in context.PPCommandes
                        where commandes.NoVendeur == NoVendeur
                        select commandes;

            return query.ToList();
        }

        private List<Panier> GetPaniersVendeurs(int NoVendeur)
        {
            var query = from articles in context.PPArticlesEnPaniers
                        where articles.NoVendeur == NoVendeur
                        orderby articles.DateCreation ascending
                        group articles by articles.NoClient into g
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

        private int GetNbVisiteurs(int NoVendeur)
        {
            var query = from visiteurs in context.PPVendeursClients
                        where visiteurs.NoVendeur == NoVendeur
                        select visiteurs;

            var reponse = query.ToList();

            return reponse.Count();
        }
    }
}