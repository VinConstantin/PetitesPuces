using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels.Gestionnaire;

namespace PetitesPuces.Controllers
{
    public class GestionnaireController : Controller
    {
        private readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();

        private readonly TimeSpan INDEX_STATS_PERIOD = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan INDEX_STATS_INCREMENT = new TimeSpan(1, 0, 0, 0);

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DonneesIndex()
        {
            IndexViewModel viewModel = new IndexViewModel
            {
                NombreDemandesVendeur = CalculerNombreDemandesVendeurs(),
                Redevances = CalculerRedevances(),
                UtilisateursInactifs = CalculerInactivite(),
            };

            return Json(viewModel);
        }

        private Dictionary<string, int> CalculerNombreDemandesVendeurs()
        {
            var demandesVendeur = 
                (from vendeurs
                    in ctxt.PPVendeurs
                where !vendeurs.Statut.HasValue ||
                      vendeurs.Statut.Value == (short) StatutCompte.INACTIF &&
                      vendeurs.DateCreation > DateTime.Today - INDEX_STATS_PERIOD
                select vendeurs).AsEnumerable().ToList();

            return ConvertirEnDictionnaire(
                demandesVendeur,
                (vendeur) => vendeur.DateCreation.GetValueOrDefault(),
                (vendeurs) => vendeurs.Count()
            );
        }

        private Dictionary<string, T> ConvertirEnDictionnaire<T, TDonnees>(
            IEnumerable<TDonnees> donnees,
            Func<TDonnees, DateTime> funcGetDate,
            Func<IEnumerable<TDonnees>, T> aggregationFunc)
        {
            DateTime cutoffDate = DateTime.Today - INDEX_STATS_PERIOD;

            T baseLine = aggregationFunc(donnees.Where(p => funcGetDate(p) < cutoffDate).ToList());
            var recent = donnees.Where(p => funcGetDate(p) >= cutoffDate).ToList();

            var donneesParPeriode = new Dictionary<string, T>();
            for (var date = new DateTime(cutoffDate.Ticks);
                date <= DateTime.Today;
                date = new DateTime(date.Ticks + INDEX_STATS_INCREMENT.Ticks))
            {
                T sommePeriode = aggregationFunc(recent.Where(p => funcGetDate(p) < cutoffDate).ToList());
                recent = recent.Where(p => funcGetDate(p) >= date).ToList();
                donneesParPeriode.Add(date.ToShortDateString(), (dynamic)sommePeriode + baseLine);

                baseLine += (dynamic)sommePeriode;
            }

            return donneesParPeriode;
        }

        private Dictionary<string, decimal> CalculerRedevances()
        {
            var paiements =
                from paiement
                    in ctxt.PPHistoriquePaiements
                select paiement;

            return ConvertirEnDictionnaire(
                paiements,
                paiement => paiement.DateVente.GetValueOrDefault(),
                tousLesPaiements => tousLesPaiements.Sum(p => p.MontantVenteAvantLivraison.GetValueOrDefault())
            );
        }

        private Dictionary<string, int> CalculerInactivite()
        {
            var utilisateursInactifs =
                ClientsInactifs().Concat(VendeursInactifs()).AsEnumerable().ToList();

            return ConvertirEnDictionnaire(
                utilisateursInactifs,
                u =>
                {
                    if (typeof(PPVendeur) == u.GetType())
                    {
                        return u.DateDerniereActivite.AddYears(1);
                    }

                    if (typeof(PPClient) == u.GetType())
                    {
                        return u.DateDerniereActivite.AddMonths(1);
                    }

                    return u.DateDerniereActivite;
                },
                u => u.Count());
        }

        private IEnumerable<IUtilisateur> ClientsInactifs()
        {
            var clientsInactifs =
                (from client
                    in ctxt.PPClients
                select client).AsEnumerable().ToList();

            return clientsInactifs.Where(c => c.DateDerniereActivite.AddMonths(1) < DateTime.Today - INDEX_STATS_PERIOD);
        }

        private IEnumerable<IUtilisateur> VendeursInactifs()
        {
            var vendeursInactifs =
                (from client
                    in ctxt.PPClients
                select client).AsEnumerable().ToList();

            return vendeursInactifs.Where(c => c.DateDerniereActivite.AddYears(1) < DateTime.Today - INDEX_STATS_PERIOD);
        }

        public ActionResult DemandesVendeur()
        {
            List<PPVendeur> viewmodel =
                (from vendeur
                    in ctxt.PPVendeurs
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
                            in ctxt.PPVendeurs
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