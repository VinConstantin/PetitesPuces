using PetitesPuces.Models;
using PetitesPuces.ViewModels.Vendeur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        // GET: Vendeur
        public ActionResult Index()
        {
            int NoVendeur = 10;

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
            int NoVendeur = 10;

            return View(GetCommandesVendeurs(NoVendeur));
        } 

        public ActionResult GestionPaniers()
        {
            int NoVendeur = 10;

            List<Panier> paniers = GetPaniersVendeurs(NoVendeur);

            paniers = paniers.Where(p => DateTime.Today.AddMonths(-6) >= p.DateCreation).ToList();

            return View(paniers);
        }

        public ActionResult GestionCatalogue()
        {
            int NoVendeur = 10;

            return View(GetProduitsVendeurs(NoVendeur));
        }

        public ActionResult InfoCommande(int No)
        {
            List<PPCommande> commandes = GetCommandesVendeurs(10);
            PPCommande model = commandes.Where(c => c.NoCommande == No).FirstOrDefault();
            return View(model);
        }

        public ActionResult Profil()
        {
            return View();
        }

        public ActionResult VisualiserPaniers(int NbMois)
        {
            List<Panier> paniers = GetPaniersVendeurs(10);
            if(NbMois != 99)
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
                        where articles.NoVendeur == 10 && articles.NoClient == NoClient
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

        private List<PPProduit> GetProduitsVendeurs(int NoVendeur)
        {
            var query = from produits in context.PPProduits
                        where produits.NoVendeur == NoVendeur
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