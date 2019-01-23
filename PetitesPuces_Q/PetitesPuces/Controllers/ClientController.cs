using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Utilisateur = "Client";
            return View();
        }
        

        public ActionResult Catalogue(string Vendeur)
        {
            ViewBag.Message = "Catalogue";
            ViewBag.Vendeur = Vendeur;
            
            /**
             * Créer des produits bidons
             */
            Random random = new Random();
            var produits = new List<Product>();
            for (int i = 1; i <= 20; i++)
            {           
                var next = random.NextDouble();

                var prix = 5.00 + (next * (1000.00 - 5.00));
                produits.Add(new Product(i, "Produit No."+i){Price = prix});
            }

            //Mettre la liste de produits dans le viewModel qui va ensuite être envoyé vers le view
            var viewModel = new CatalogueViewModel
            {
                Produits = produits
            };

            return View(viewModel);
        }
        public ActionResult MonPanier(string No)
        {
            ViewBag.Message = "Votre panier No. "+No;
            
            return View();
        }
        public ActionResult Commande()
        {
            
            
            return View();
        }

        public ActionResult Profil()
        {
            return View();
        }
        public ActionResult modificationMDP()
        {
            return View();
        }

    }
}
