using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Gestionnaire;

namespace PetitesPuces.Controllers
{
    public class GestionnaireController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Redevances()
        {
            return View();
        }

        public ActionResult DemandesVendeur()
        {
            return View();
        }

        public ActionResult Inactivite()
        {
            var viewModel = new InactiviteViewModel
            {
                ClientsInactifs = new List<Client>
                {
                    new Client
                    {
                        NoClient = 1,
                        AdresseEmail = "test@test.ca",
                        DateDerniereConnexion = DateTime.Today,
                        Nom = "Inactif",
                        Prenom = "Client",
                    }
                },
                VendeursInactifs = new List<Vendeur>()
                {
                    new Vendeur
                    {
                        NoVendeur = 1,
                        Nom = "Nom",
                        NomAffaires =  "Magasin",
                        Prenom = "Prenom",
                    }
                }
            };

            return View(viewModel);
        }

        public ActionResult Statistiques()
        {
            return View();
        }
    }
}