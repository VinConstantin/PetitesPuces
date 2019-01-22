using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Catalogue()
        {
            ViewBag.Message = "Catalogue";
            Random random = new Random();
            var produits = new List<Product>();
            for (int i = 1; i <= 20; i++)
            {           
                var next = random.NextDouble();

                var prix = 5.00 + (next * (1000.00 - 5.00));
                produits.Add(new Product(i, "Produit No."+i){Price = prix});
            }

            var viewModel = new CatalogueViewModel
            {
                Produits = produits
            };

            return View(viewModel);
        }
    }
}