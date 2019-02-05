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

            List<PPCommande> commandes = getCommandesVendeurs(NoVendeur);

            commandes = commandes.Where(c => c.Statut == 'T').ToList();

            var viewModel = new AccueilVendeurViewModel
            {
                Commandes = commandes,
                Paniers = getPaniersVendeurs(NoVendeur),
                NbVisites = getNbVisiteurs(NoVendeur)
            };
            return View(viewModel);
        }

        public ActionResult GestionCommandes()
        {
            int NoVendeur = 10;

            List<PPCommande> commandes = getCommandesVendeurs(NoVendeur);

            return View(commandes);
        } 

        public ActionResult GestionPaniers()
        {
            int NoVendeur = 10;

            List<Panier> paniers = getPaniersVendeurs(NoVendeur);

            paniers = paniers.Where(p => DateTime.Today.AddMonths(-6) >= p.DateCreation).ToList();

            return View(paniers);
        }

        public ActionResult GestionCatalogue()
        {
            /**
             * Créer des produits bidons
             */
            Random random = new Random();
            var produits = new List<Produit>();
            for (int i = 1; i <= 20; i++)
            {
                var next = random.NextDouble();

                var prix = 5.00 + (next * (1000.00 - 5.00));
                produits.Add(new Produit(i, "Produit No." + i) { Price = prix });
            }

            return View(produits);
        }

        public ActionResult InfoCommande(int No)
        {
            List<PPCommande> commandes = getCommandesVendeurs(10);
            PPCommande model = commandes.Where(c => c.NoCommande == No).FirstOrDefault();
            return View(model);
        }

        public ActionResult Profil()
        {
            return View();
        }

        public ActionResult VisualiserPaniers(int NbMois)
        {
            List<Panier> paniers = getPaniersVendeurs(10);
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

        private List<PPCommande> getCommandesVendeurs(int NoVendeur)
        {
            var query = from commandes in context.PPCommandes
                        where commandes.NoVendeur == NoVendeur
                        select commandes;

            return query.ToList();
        }

        private List<Panier> getPaniersVendeurs(int NoVendeur)
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
                    NomClient = pan.FirstOrDefault().PPClient.Nom,
                    NbItems = (int)pan.Sum(g => g.NbItems),
                    DateCreation = (DateTime)pan.FirstOrDefault().DateCreation,
                    CoutTotal = pan.Sum(g => (double)(g.NbItems * g.PPProduit.PrixDemande))
                };
                lstPaniers.Add(panier);
            }

            return lstPaniers;
        }

        private int getNbVisiteurs(int NoVendeur)
        {
            var query = from visiteurs in context.PPVendeursClients
                        where visiteurs.NoVendeur == NoVendeur
                        select visiteurs;

            var reponse = query.ToList();

            return reponse.Count();
        }
    }
}