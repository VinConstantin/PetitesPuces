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

        public ActionResult DemandesVendeur()
        {
            var viewmodel = new List<Vendeur>()
            {
                new Vendeur
                {
                    NoVendeur = 1,
                    Nom = "Nom",
                    NomAffaires = "Magasin",
                    Prenom = "Prenom",
                    DateCreation = DateTime.Today,
                }
            };

            return View(viewmodel);
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
                        AdresseEmail = "vendeur@company.ca"
                    }
                }
            };

            viewModel.UtilsRecherche = new List<IPersonne>
            {
                viewModel.ClientsInactifs[0],
                viewModel.VendeursInactifs[0],
            };

            return View(viewModel);
        }

        public ActionResult Statistiques()
        {
            var viewModel = new StatsViewModel
            {
                TotalVendeurs = 10,
            };

            return View(viewModel);
        }
    }
}