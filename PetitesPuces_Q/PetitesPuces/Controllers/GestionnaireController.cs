using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using System.Xml.Linq;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels.Gestionnaire;

namespace PetitesPuces.Controllers
{
    [System.Web.Mvc.RoutePrefix("Gestionnaire")]
    public class GestionnaireController : Controller
    {
        private readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();

        private readonly TimeSpan INDEX_STATS_PERIOD = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan INDEX_STATS_INCREMENT = new TimeSpan(1, 0, 0, 0);

        public GestionnaireController()
        {
            searchFuncs = 
                new Dictionary<string, Func<string, List<IUtilisateur>>>
                {
                    {RolesUtil.UTIL, GetUtilsAvecNom},
                    {RolesUtil.VEND, GetVendeursAvecNom},
                    {RolesUtil.CLIENT, GetClientsAvecNom},
                };
        }
        
        #region Index    
        public ActionResult Index()
        {
            return View(GetCategories());
        }

        private List<Tuple<PPCategory, bool>> GetCategories()
        {
            return
                (from categorie
                        in ctxt.PPCategories
                    select new Tuple<PPCategory, bool> (categorie, categorie.PPProduits.Any())).AsEnumerable().ToList();
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("Categories")]
        public ActionResult HttpGetCategories()
        {
            return PartialView("Gestionnaire/_GestionCategories", GetCategories());
        }

        [System.Web.Http.HttpPut]
        [System.Web.Mvc.Route("Categories")]
        public ActionResult MAJCategorie([FromBody]PPCategory categorie)
        {
            try
            {
                var categorieBd =
                    (from cat
                        in ctxt.PPCategories
                     where cat.NoCategorie == categorie.NoCategorie
                        select cat).First();

                categorieBd.Description = categorie.Description;
                categorieBd.Details = categorie.Details;

                ctxt.SubmitChanges();

                return PartialView("Gestionnaire/_GestionCategories", GetCategories());
            }
            catch (InvalidOperationException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }          
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("Categories")]
        public ActionResult AjoutCategorie(PPCategory categorie)
        {
            var categories = 
                from cat
                    in ctxt.PPCategories
                where cat.Description == categorie.Description
                select cat;

            if (categories.Any()) return HttpBadRequest();

            var nextId =
                (from cat
                        in ctxt.PPCategories
                    select cat.NoCategorie).Max() + 10;

            categorie.NoCategorie = nextId;
            ctxt.PPCategories.InsertOnSubmit(categorie);
            ctxt.SubmitChanges();

            return PartialView("Gestionnaire/_GestionCategories", GetCategories());
        }

        [System.Web.Mvc.HttpDelete]
        [System.Web.Mvc.Route("Categories/{id}")]
        public ActionResult SuppressionCategorie(int id)
        {
            try
            {
                var categorieBd =
                    (from cat
                            in ctxt.PPCategories
                        where cat.NoCategorie == id
                        select cat).First();

                ctxt.PPCategories.DeleteOnSubmit(categorieBd);
                ctxt.SubmitChanges();

                return PartialView("Gestionnaire/_GestionCategories", GetCategories());
            }
            catch (InvalidOperationException)
            {
                return HttpBadRequest();
            }
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult DonneesIndex()
        {
            IndexStats stats = new IndexStats
            {
                NombreDemandesVendeur = CalculerNombreDemandesVendeurs(),
                Redevances = CalculerRedevances(),
                UtilisateursInactifs = CalculerInactivite(),
            };

            return Json(stats);
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
        #endregion

        #region DemandesVendeur
        public ActionResult DemandesVendeur(int? id)
        {
            if (id.HasValue) return DemandesVendeur(id.Value);

            return DemandesVendeur();
        }

        private ActionResult DemandesVendeur(int id)
        {
            try
            {
                PPVendeur demandeVendeur =
                    (from vendeur
                            in ctxt.PPVendeurs
                        where vendeur.NoVendeur == id
                        select vendeur).First();

                return PartialView("Gestionnaire/Demandes/_DetailsVendeur", demandeVendeur);
            }
            catch (InvalidOperationException)
            {
                return HttpNotFound();
            }
        }
        
        private ActionResult DemandesVendeur()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView("Gestionnaire/Demandes/_ListeDemandesVendeur", GetDemandes());
            }
            
            return View(GetDemandes());
        }
        
        private List<PPVendeur> GetDemandes()
        {
            return (from vendeur
                    in ctxt.PPVendeurs
                where vendeur.Statut == 0
                select vendeur).AsEnumerable().ToList();
        }
        
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GererDemande/Details/{id}")]
        public ActionResult GererDemandeVendeurDetails([FromUri]int id)
        {
            try
            {
                return PartialView("Gestionnaire/Demandes/_GestionDemande",
                    new Tuple<int, TypesModal>(id, TypesModal.DETAILS));
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }
        }
        
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GererDemande/Accepter/{id}")]
        public ActionResult GererDemandeVendeurAccepter([FromUri]int id)
        {
            return PartialView("Gestionnaire/Demandes/_GestionDemande", new Tuple<int, TypesModal>(id, TypesModal.ACCEPTER));
        }
        
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("GererDemande/Refuser/{id}")]
        public ActionResult GererDemandeVendeurRefuser([FromUri]int id)
        {
            return PartialView("Gestionnaire/Demandes/_GestionDemande", new Tuple<int, TypesModal>(id, TypesModal.REFUSER));
        }

        public enum TypesModal
        {
            DETAILS,
            ACCEPTER,
            REFUSER
        }
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("GererDemande/{id}")]
        public ActionResult GererDemandeVendeur([FromUri]int id, [FromBody] ReponseDemandeVendeur reponse)
        {
            try
            {
                PPVendeur vendeurAAccepter =
                    (from vendeur
                            in ctxt.PPVendeurs
                        where vendeur.NoVendeur == id
                        select vendeur).First();
                
                if (reponse.Accepte)
                {
                    vendeurAAccepter.Statut = (int) StatutCompte.ACTIF;
                }
                else
                {
                    ctxt.PPVendeurs.DeleteOnSubmit(vendeurAAccepter);
                }
                
                ctxt.SubmitChanges();

                return PartialView("Gestionnaire/Demandes/_ListeDemandesVendeur", GetDemandes());
            }
            catch (InvalidOperationException)
            {
                return HttpBadRequest();
            }
        }
        #endregion
        
        public ActionResult Inactivite(string depuisClient = "T", string depuisVendeur = "T", string typeUtilisateur = "Utilisateur")
        {
            TimeSpan periodeClient = ParsePeriode(depuisClient);
            TimeSpan periodeVendeur = ParsePeriode(depuisVendeur);
            
            var viewModel = new InactiviteViewModel
            {
                ClientsInactifs = ClientsInactifsPour(periodeClient),
                VendeursInactifs = VendeursInactifsPour(periodeVendeur),
                UtilsRecherche = RechercheUtilisateurs(typeUtilisateur),
            };

            return View(viewModel);
        }

        private List<PPClient> ClientsInactifsPour(TimeSpan periode)
        {
            return
                (from client
                        in ctxt.PPClients
                    select client)
                .AsEnumerable()
                .Where(UtilInactifDepuis<PPClient>(periode))
                .ToList();
        }

        private Func<T, bool> UtilInactifDepuis<T>(TimeSpan periode) where T: IUtilisateur
        {
            return utilisateur =>
            {
                var tempsInactivite = DateTime.Now - utilisateur.DateDerniereActivite;
                return tempsInactivite > periode;
            };
        }
        
        private List<PPVendeur> VendeursInactifsPour(TimeSpan periode)
        {
            return
                (from vendeur
                        in ctxt.PPVendeurs
                    select vendeur)
                .AsEnumerable()
                .Where(UtilInactifDepuis<PPVendeur>(periode))
                .ToList();
        }

        private readonly Dictionary<string, Func<string, List<IUtilisateur>>> searchFuncs;
        
        private List<IUtilisateur> RechercheUtilisateurs(string typeUtilisateur)
        {
            return searchFuncs[typeUtilisateur]("").ToList();
        }

        [System.Web.Mvc.Route("Inactivite/Liste/{typeUtilisateur}")]
        public ActionResult RefreshInactifs(string typeUtilisateur)
        {            
            if (typeUtilisateur == RolesUtil.UTIL)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisablePersonne", Tuple.Create(RechercheUtilisateurs(typeUtilisateur), true));
            } 
            else if (typeUtilisateur == RolesUtil.VEND)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisableVendeur", Tuple.Create(RechercheUtilisateurs(typeUtilisateur).Cast<PPVendeur>().ToList(), true));
            }
            else if (typeUtilisateur == RolesUtil.CLIENT)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisableClient", Tuple.Create(RechercheUtilisateurs(typeUtilisateur).Cast<PPClient>().ToList(), true)); 
            }
            else
            {
                return HttpBadRequest();
            }
        }
       
        private List<IUtilisateur> GetUtilsAvecNom(string nom)
        {
            return GetClientsAvecNom(nom).Concat(GetVendeursAvecNom(nom)).ToList();
        }

        private List<IUtilisateur> GetClientsAvecNom(string nom)
        {
            return
                (from util
                    in ctxt.PPClients
                where (util.Nom + util.Prenom).Contains(nom)
                select util).Cast<IUtilisateur>().ToList();
        }

        private List<IUtilisateur> GetVendeursAvecNom(string nom)
        {
            return
                (from util
                    in ctxt.PPVendeurs
                where (util.Nom + util.Prenom).Contains(nom)
                select util).Cast<IUtilisateur>().ToList();
        }
        
        private TimeSpan ParsePeriode(string depuis)
        {
            if (depuis == "T")
            {
                return new TimeSpan(DateTime.Now.Ticks);
            }
            else
            {
                var nb = int.Parse(depuis[0].ToString());

                DateTime spanEnd = DateTime.Now;
                DateTime spanStart = depuis[1] == 'm' ? spanEnd.AddMonths(-nb) : spanEnd.AddYears(-nb);
                
                return spanEnd - spanStart;
            }
        }

        public ActionResult Statistiques()
        {
            var viewModel = new StatsViewModel
            {
                TotalVendeurs = 10,
            };

            return View(viewModel);
        }

        public ActionResult Redevances(int? id)
        {
            if (id.HasValue) return Redevances(id.Value);

            return Redevances();
        }
        
        private ActionResult Redevances()
        {
            var viewModel = new RedevancesViewModel
            {
                Redevances = GetRedevances(),
                Vendeurs = GetVendeursRedevances(),
            };

            return View(viewModel);
        }
        
        private ActionResult Redevances(int id)
        {
            try
            {
                var redevances =
                    (from paiement
                            in ctxt.PPHistoriquePaiements
                        where paiement.NoVendeur == id && paiement.Redevance > 0
                        select paiement).ToList();

                var vendeur =
                    (from v
                            in ctxt.PPVendeurs
                        where v.NoVendeur == id
                        select v).First();

                var viewModel = new DetailsRedevances
                {
                    Vendeur = vendeur,
                    Paiements = redevances,
                };

                return PartialView("Gestionnaire/Redevances/_DetailsRedevances", viewModel);
            }
            catch (InvalidOperationException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
           
        }
        
        [System.Web.Mvc.Route("Redevances/Dues")]
        public ActionResult RefreshRedevances()
        {
            return PartialView("Gestionnaire/Redevances/_ListeRedevances", GetRedevances());
        }

        [System.Web.Mvc.Route("Redevances/Modifiables")]
        public ActionResult RefreshVendeurRedevances()
        {
            return PartialView("Gestionnaire/Redevances/_ListeVendeurRedevance", GetVendeursRedevances());
        }

        [System.Web.Mvc.HttpPatch]
        [System.Web.Mvc.Route("Redevances/Vendeur/{idVendeur}")]
        public ActionResult UpdateRedevancesVendeur([FromUri]int idVendeur, [FromBody] decimal pourcentage)
        {
            try
            {
                var vendeurAUpdate =
                    (from vendeur
                            in ctxt.PPVendeurs
                        where vendeur.NoVendeur == idVendeur
                        select vendeur).First();

                vendeurAUpdate.Pourcentage = pourcentage;
                
                ctxt.SubmitChanges();
                
                return RefreshVendeurRedevances();
            }
            catch (InvalidOperationException e)
            {
                return HttpBadRequest();
            }
        }
        
        private List<Redevance> GetRedevances()
        {
            return
                (from vendeur
                    in ctxt.PPVendeurs
                join paiement in ctxt.PPHistoriquePaiements 
                    on vendeur.NoVendeur equals paiement.NoVendeur
                    into paiements
                select new Redevance
                {
                    Solde = paiements.Select(p => p.Redevance).Where(r => r > 0).Sum() ?? 0,
                    Vendeur = vendeur,
                }).ToList();
        }

        private List<PPVendeur> GetVendeursRedevances()
        {
            return
                (from vendeur
                    in ctxt.PPVendeurs
                join paiement in ctxt.PPHistoriquePaiements 
                    on vendeur.NoVendeur equals paiement.NoVendeur
                    into paiements
                where 
                    paiements.All(c => c.NoVendeur != vendeur.NoVendeur)
                select vendeur)
                .ToList();
        }

        private HttpStatusCodeResult HttpBadRequest()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        private ActionResult XMLBD()
        {
           
            return View();

        }
    }
}