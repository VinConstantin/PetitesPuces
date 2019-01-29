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

            var viewModel = new AccueilVendeurViewModel
            {
                Commandes = getCommandesVendeurs(NoVendeur),
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
            List<Panier> viewModel = new List<Panier>
                {
                    new Panier
                    {
                        NomClient = "Antoinette",
                        DateCreation = new DateTime(2018, 03, 26),
                        NbItems = 7
                    },
                    new Panier
                    {
                        NomClient = "Vincent",
                        DateCreation = new DateTime(2018, 08, 02),
                        NbItems = 5
                    },
                    new Panier
                    {
                        NomClient = "Simon",
                        DateCreation = new DateTime(2018, 10, 06),
                        NbItems = 3
                    },
                    new Panier
                    {
                        NomClient = "Alain",
                        DateCreation = new DateTime(2018, 11, 10),
                        NbItems = 4
                    },
                    new Panier
                    {
                        NomClient = "Raph",
                        DateCreation = new DateTime(2019, 01, 13),
                        NbItems = 15
                    },
                    new Panier
                    {
                        NomClient = "Samuel",
                        DateCreation = new DateTime(2019, 01, 16),
                        NbItems = 9
                    }
                };
            return View(viewModel);
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

        public ActionResult InfoCommande(string No)
        {
            ViewBag.No = No;
            return View();
        }

        public ActionResult Profil()
        {
            return View();
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