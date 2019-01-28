using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Gestionnaire;

namespace PetitesPuces.Controllers
{
    public class GestionnaireController : Controller
    {
        private BDPetitesPucesDataContext dataContext = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DemandesVendeur()
        {
            List<PPVendeur> viewmodel =
                (from vendeur
                    in dataContext.PPVendeurs
                where vendeur.Statut == 0
                select vendeur).ToList();

            return View(viewmodel);
        }

        public ActionResult DetailsDemande(int id)
        {
            try
            {
                PPVendeur demandeVendeur =
                    (from vendeur
                            in dataContext.PPVendeurs
                        where vendeur.NoVendeur == id
                        select vendeur).First();

                return PartialView("Gestionnaire/_DetailsVendeur", demandeVendeur);
            }
            catch (InvalidOperationException)
            {
                return HttpNotFound();
            }
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

            viewModel.UtilsRecherche = new List<IUtilisateur>
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

        public ActionResult Redevances()
        {
            var viewModel = new List<Redevance>
            {
                new Redevance
                {
                    EnSouffranceDepuis = DateTime.Today,
                    Solde = 1000,
                    Vendeur = new PPVendeur
                    {
                        NomAffaires = "Vendeur boi",
                        Pourcentage = 50,
                    }
                }
            };

            return View(viewModel);
        }
    }
}