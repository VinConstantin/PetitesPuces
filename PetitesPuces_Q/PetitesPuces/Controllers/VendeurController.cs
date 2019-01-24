using PetitesPuces.Models;
using PetitesPuces.ViewModels.VMVendeur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Controllers
{
    public class VendeurController : Controller
    {
        // GET: Vendeur
        public ActionResult Index()
        {
            var viewModel = new AccueilVendeurViewModel
            {
                Commandes = new List<Commande>
                {
                    new Commande
                    {
                        NoCommande = 1,
                        NomClient = "Antoinette",
                        DateCommande = new DateTime(2018, 12, 28),
                        CoutLivraison = 14.99,
                        Type = "Express",
                        PoidsTotal = 25.33,
                        Statut = 'P',
                        TotalAvantTaxes = 99.00
                    },
                    new Commande
                    {
                        NoCommande = 2,
                        NomClient = "Vincent",
                        DateCommande = new DateTime(2019, 01, 05),
                        CoutLivraison = 4.99,
                        Type = "Standard",
                        PoidsTotal = 99.33,
                        Statut = 'P',
                        TotalAvantTaxes = 49.99
                    },
                    new Commande
                    {
                        NoCommande = 3,
                        NomClient = "Simon",
                        DateCommande = new DateTime(2019, 01, 09),
                        CoutLivraison = 24.99,
                        Type = "Express",
                        PoidsTotal = 12.90,
                        Statut = 'P',
                        TotalAvantTaxes = 199.99
                    },
                    new Commande
                    {
                        NoCommande = 4,
                        NomClient = "Antoinette",
                        DateCommande = new DateTime(2019, 01, 11),
                        CoutLivraison = 4.99,
                        Type = "Standard",
                        PoidsTotal = 18.72,
                        Statut = 'P',
                        TotalAvantTaxes = 39.00
                    },
                    new Commande
                    {
                        NoCommande = 5,
                        NomClient = "Vincent",
                        DateCommande = new DateTime(2019, 01, 15),
                        CoutLivraison = 9.99,
                        Type = "Standard",
                        PoidsTotal = 49.39,
                        Statut = 'P',
                        TotalAvantTaxes = 79.99
                    },
                    new Commande
                    {
                        NoCommande = 6,
                        NomClient = "Simon",
                        DateCommande = new DateTime(2019, 01, 20),
                        CoutLivraison = 14.99,
                        Type = "Express",
                        PoidsTotal = 46.15,
                        Statut = 'P',
                        TotalAvantTaxes = 189.99
                    }
                },

                Paniers = new List<Panier>
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
                        DateCreation = new DateTime(2018, 12, 02),
                        NbItems = 5
                    },
                    new Panier
                    {
                        NomClient = "Simon",
                        DateCreation = new DateTime(2019, 01, 06),
                        NbItems = 3
                    },
                    new Panier
                    {
                        NomClient = "Alain",
                        DateCreation = new DateTime(2019, 01, 10),
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
                },

                NbVisites = 3
            };
            return View(viewModel);
        }

        public ActionResult GestionCommandes()
        {
            List<Commande> viewModel = new List<Commande>
            {
                    new Commande
                    {
                        NoCommande = 1,
                        NomClient = "Antoinette",
                        DateCommande = new DateTime(2018, 12, 28),
                        CoutLivraison = 14.99,
                        Type = "Express",
                        PoidsTotal = 25.33,
                        Statut = 'L',
                        TotalAvantTaxes = 99.00
                    },
                    new Commande
                    {
                        NoCommande = 2,
                        NomClient = "Vincent",
                        DateCommande = new DateTime(2019, 01, 05),
                        CoutLivraison = 4.99,
                        Type = "Standard",
                        PoidsTotal = 99.33,
                        Statut = 'L',
                        TotalAvantTaxes = 49.99
                    },
                    new Commande
                    {
                        NoCommande = 3,
                        NomClient = "Simon",
                        DateCommande = new DateTime(2019, 01, 09),
                        CoutLivraison = 24.99,
                        Type = "Express",
                        PoidsTotal = 12.90,
                        Statut = 'L',
                        TotalAvantTaxes = 199.99
                    },
                    new Commande
                    {
                        NoCommande = 4,
                        NomClient = "Antoinette",
                        DateCommande = new DateTime(2019, 01, 11),
                        CoutLivraison = 4.99,
                        Type = "Standard",
                        PoidsTotal = 18.72,
                        Statut = 'P',
                        TotalAvantTaxes = 39.00
                    },
                    new Commande
                    {
                        NoCommande = 5,
                        NomClient = "Vincent",
                        DateCommande = new DateTime(2019, 01, 15),
                        CoutLivraison = 9.99,
                        Type = "Standard",
                        PoidsTotal = 49.39,
                        Statut = 'P',
                        TotalAvantTaxes = 79.99
                    },
                    new Commande
                    {
                        NoCommande = 6,
                        NomClient = "Simon",
                        DateCommande = new DateTime(2019, 01, 20),
                        CoutLivraison = 14.99,
                        Type = "Express",
                        PoidsTotal = 46.15,
                        Statut = 'P',
                        TotalAvantTaxes = 189.99
                    }
            };
            return View(viewModel);
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
    }
}