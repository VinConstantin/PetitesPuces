
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Vendeur;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels;

namespace PetitesPuces.Controllers
{
#if !DEBUG
        [Securise(RolesUtil.VEND)]
#endif
    public class VendeurController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        private int NoVendeur = Convert.ToInt32(SessionUtilisateur.UtilisateurCourant.No);

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
            long NoProduit = Produits.Count() != 0
                ? Produits.LastOrDefault().NoProduit + 1
                : long.Parse(NoVendeur + "00001");

            var viewModel = new GestionCatalogueViewModel
            {
                NoProduit = NoProduit,
                Produits = Produits,
                Categories = GetCategories()
            };

            return View(viewModel);
        }

        public ActionResult InfoCommande(int No)
        {
            List<PPCommande> commandes = GetCommandesVendeurs(10);
            PPCommande model = commandes.Where(c => c.NoCommande == No).FirstOrDefault();
            return View(model);
        }

        [HttpGet]
        public ActionResult Profil()
        {
            var objVendeur = (from unVendeur in context.PPVendeurs
                where unVendeur.NoVendeur == NoVendeur
                select unVendeur).FirstOrDefault();

            ModiProfilVendeur modiProfilVendeur = new ModiProfilVendeur
            {
                NomAffaires = objVendeur.NomAffaires,
                Nom = objVendeur.Nom,
                Prenom = objVendeur.Prenom,
                Rue = objVendeur.Rue,
                Ville = objVendeur.Ville,
                Province = objVendeur.Province,
                Pays = objVendeur.Pays,
                CodePostal = objVendeur.CodePostal.ToUpper(),
                Tel1 = objVendeur.Tel1,
                Tel2 = objVendeur.Tel2,
                PoidsMaxLivraison = Convert.ToInt32(objVendeur.PoidsMaxLivraison),
                LivraisonGratuite = Convert.ToInt32(objVendeur.LivraisonGratuite),
                Taxes = Convert.ToBoolean(objVendeur.Taxes)
            };
            return View(modiProfilVendeur);
        }

        [HttpPost]
        public ActionResult Profil(ModiProfilVendeur modiProfilVendeur)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var objVendeur = (from unVendeur in context.PPVendeurs
                        where unVendeur.NoVendeur == NoVendeur
                        select unVendeur).FirstOrDefault();

                    if (modiProfilVendeur.NomAffaires != "") objVendeur.NomAffaires = modiProfilVendeur.NomAffaires;
                    if (modiProfilVendeur.Nom != "") objVendeur.Nom = modiProfilVendeur.Nom;
                    if (modiProfilVendeur.Prenom != "") objVendeur.Prenom = modiProfilVendeur.Prenom;
                    if (modiProfilVendeur.Rue != "") objVendeur.Rue = modiProfilVendeur.Rue;
                    if (modiProfilVendeur.Ville != "") objVendeur.Ville = modiProfilVendeur.Ville;
                    if (modiProfilVendeur.Province != "") objVendeur.Province = modiProfilVendeur.Province;
                    if (modiProfilVendeur.Pays != "") objVendeur.Pays = modiProfilVendeur.Pays;
                    if (modiProfilVendeur.CodePostal != "") objVendeur.CodePostal = modiProfilVendeur.CodePostal;
                    if (modiProfilVendeur.Tel1 != "") objVendeur.Tel1 = modiProfilVendeur.Tel1;
                    if (modiProfilVendeur.Tel2 != "") objVendeur.Tel2 = modiProfilVendeur.Tel2;
                    if (modiProfilVendeur.PoidsMaxLivraison != null)
                        objVendeur.PoidsMaxLivraison = modiProfilVendeur.PoidsMaxLivraison;
                    if (modiProfilVendeur.LivraisonGratuite != null)
                        objVendeur.LivraisonGratuite = modiProfilVendeur.LivraisonGratuite;
                    if (modiProfilVendeur.Taxes != null) objVendeur.Taxes = modiProfilVendeur.Taxes;

                    context.SubmitChanges();
                    ViewBag.SuccessMessage = "Modification réussite!";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                }
            }

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

        public ActionResult ModalSupprimerProduit(int NoProduit) //TODO
        {
            return PartialView("Vendeur/ModalSupprimerProduit", NoProduit);
        }

        public ActionResult Evaluations(int NoProduit)
        {
            List<PPEvaluation> evaluations = (from e in context.PPEvaluations
                where e.NoProduit == NoProduit && e.PPProduit.NoVendeur == NoVendeur
                select e).ToList();

            PPProduit produit = (from p in context.PPProduits
                where p.NoProduit == NoProduit
                select p).FirstOrDefault();
            
            if(produit==null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            
            return View(new Tuple<List<PPEvaluation>,PPProduit>(evaluations,produit));
        }

        public void ModifierProduit() //TODO
        {
            NameValueCollection nvc = Request.Form;


            PPProduit produit = new PPProduit
            {
                NoProduit = long.Parse(nvc["NoProduit"]),
                DateCreation = DateTime.Parse(nvc["DateCreation"]),
                NoCategorie = int.Parse(nvc["NoCategorie"]),
                NombreItems = short.Parse(nvc["NombreItems"]),
                Nom = nvc["Nom"],
                PrixVente = decimal.Parse(nvc["PrixVente"]),
                PrixDemande = decimal.Parse(nvc["PrixDemande"]),
                Poids = decimal.Parse(nvc["Poids"]),
                Description = nvc["Description"],
                Disponibilité = nvc["Disponibilite"] == "on" ? true : false,
                NoVendeur = NoVendeur,
                DateMAJ = DateTime.Parse(nvc["DateCreation"])
            };

            DateTime date;
            if (DateTime.TryParse(nvc["DateVente"], out date)) produit.DateVente = date;

            HttpPostedFileBase hpfb = Request.Files.Get(0);
            if (hpfb.FileName != "")
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/images/produits/";
                string filename = produit.NoProduit.ToString() + Path.GetExtension(hpfb.FileName);
                produit.Photo = filename;
                hpfb.SaveAs(Path.Combine(path, filename));
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
        }

        public void SupprimerProduit(int NoProduit) //TODO
        {
            var query = from produit in context.PPProduits
                where produit.NoProduit == NoProduit
                select produit;

            context.PPProduits.DeleteOnSubmit(query.FirstOrDefault());

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
                PrixVente = decimal.Parse(nvc["PrixVente"]),
                PrixDemande = decimal.Parse(nvc["PrixDemande"]),
                Poids = decimal.Parse(nvc["Poids"]),
                Description = nvc["Description"],
                Disponibilité = nvc["Disponibilite"] == "on" ? true : false,
                NoVendeur = NoVendeur,
                DateMAJ = DateTime.Parse(nvc["DateCreation"])
            };

            DateTime date;
            if (DateTime.TryParse(nvc["DateVente"], out date)) produit.DateVente = date;

            HttpPostedFileBase hpfb = Request.Files.Get(0);
            if (hpfb.FileName != "")
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "/images/produits/";
                string filename = produit.NoProduit.ToString() + Path.GetExtension(hpfb.FileName);
                produit.Photo = filename;
                hpfb.SaveAs(Path.Combine(path, filename));
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
                group articles by articles.NoClient
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

        private int GetNbVisiteurs(int NoVendeur)
        {
            var query = from visiteurs in context.PPVendeursClients
                where visiteurs.NoVendeur == NoVendeur
                select visiteurs;

            var reponse = query.ToList();

            return reponse.Count();
        }

        [HttpGet]
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

        [HttpPost]
        public ActionResult modificationMDP(ModificationMDP modificationMdp)
        {
            var unVendeur=(from vendeur in context.PPVendeurs
                where vendeur.NoVendeur==NoVendeur
                      select vendeur).First();

            bool motDePasseValide = modificationMdp.ancienMDP == unVendeur.MotDePasse;
            
            if (ModelState.IsValid)
            {
                if (!motDePasseValide)
                {
                    ModelState.AddModelError(string.Empty,"L'ancien mot de passe est invalide!");
                    return View(modificationMdp);
                }

                unVendeur.MotDePasse = modificationMdp.motDePass;
                try
                {
                    context.SubmitChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception e)
                {
                    
                }
            }

            return View(modificationMdp);
        }
    }
}