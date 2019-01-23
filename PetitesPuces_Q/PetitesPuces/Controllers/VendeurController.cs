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
            var viewModel = new CommandesViewModel
            {
                Commandes = new List<Commande>
                {
                    new Commande
                    {
                        NoCommande = 1,
                        NomClient = "Antoinette",
                        DateCommande = new DateTime(2018, 12, 28),
                        Type = "Express",
                        TotalAvantTaxes = 99.00
                    },
                    new Commande
                    {
                        NoCommande = 2,
                        NomClient = "Vincent",
                        DateCommande = new DateTime(2019, 01, 05),
                        Type = "Standard",
                        TotalAvantTaxes = 49.99
                    },
                    new Commande
                    {
                        NoCommande = 3,
                        NomClient = "Simon",
                        DateCommande = new DateTime(2019, 01, 09),
                        Type = "Express",
                        TotalAvantTaxes = 199.99
                    }
                }
            };
            return View(viewModel);
        }
    }
}