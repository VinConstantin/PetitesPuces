using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels.Gestionnaire;

namespace PetitesPuces.Controllers
{
    #if !DEBUG
        [Securise(RolesUtil.ADMIN)]
    #endif
    [System.Web.Mvc.RoutePrefix("Gestionnaire")]
    public class GestionnaireController : Controller
    {
        private readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();

        private readonly TimeSpan INDEX_STATS_PERIOD = new TimeSpan(7, 0, 0, 0);
        private readonly TimeSpan INDEX_STATS_INCREMENT = new TimeSpan(1, 0, 0, 0);
        private readonly DateTime BEGINNING = new DateTime(2000, 01, 01);

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
                (vendeurs) => vendeurs.Count(),
                INDEX_STATS_PERIOD
            );
        }

        private Dictionary<string, T> ConvertirEnDictionnaire<T, TDonnees>(
            IEnumerable<TDonnees> donnees,
            Func<TDonnees, DateTime> funcGetDate,
            Func<IEnumerable<TDonnees>, T> aggregationFunc,
            TimeSpan periode,
            TimeSpan increment
            )
        {
            DateTime cutoffDate = DateTime.Today - periode;

            T baseline = aggregationFunc(donnees.Where(p => funcGetDate(p) < cutoffDate).ToList());
            var recent = donnees.Where(p => funcGetDate(p) >= cutoffDate).ToList();

            var donneesParPeriode = new Dictionary<string, T>();
            for (var date = new DateTime(cutoffDate.Ticks);
                date <= DateTime.Today;
                date = new DateTime(date.Ticks + increment.Ticks))
            {
                T sommePeriode = aggregationFunc(recent.Where(p => funcGetDate(p) < date).ToList());
                recent = recent.Where(p => funcGetDate(p) >= date).ToList();
                donneesParPeriode.Add(date.ToShortDateString(), (dynamic)sommePeriode + baseline);

                baseline += (dynamic)sommePeriode;
            }

            return donneesParPeriode;
        }

        private Dictionary<string, T> ConvertirEnDictionnaire<T, TDonnees>(
            IEnumerable<TDonnees> donnees,
            Func<TDonnees, DateTime> funcGetDate,
            Func<IEnumerable<TDonnees>, T> aggregationFunc,
            TimeSpan periode
        )
        {
            return ConvertirEnDictionnaire(donnees, funcGetDate, aggregationFunc, periode, INDEX_STATS_INCREMENT);
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
                tousLesPaiements => tousLesPaiements.Sum(p => p.MontantVenteAvantLivraison.GetValueOrDefault()),
                INDEX_STATS_PERIOD
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
                u => u.Count(),
                INDEX_STATS_PERIOD);
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
                    orderby vendeur.DateCreation descending
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
                    orderby vendeur.DateCreation descending
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
            catch (Exception)
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
               
        #region Inactivite
        [System.Web.Mvc.Route("Inactivite")]
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
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("Inactivite/RendreInactif")]
        public ActionResult SupprimerById([FromBody]long[] nosASupprimer)
        {
            try
            {
                SupprimerUtilisateurs(nosASupprimer);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
            }
        }

        private void SupprimerUtilisateurs(long[] ids)
        {
            SupprimerClients(ids);
            SupprimerVendeurs(ids);
            
            ctxt.SubmitChanges();
        }

        private void SupprimerClients(long[] ids)
        { 
            var clientsADelete =
                from client
                    in ctxt.PPClients
                where ids.Contains(client.NoClient)
                select client;

            foreach (var client in clientsADelete)
            {
                if (!client.PPVendeursClients.Any())
                {
                    ctxt.PPClients.DeleteOnSubmit(client);
                }
                else if (!client.PPCommandes.Any())
                {
                    ctxt.PPVendeursClients.DeleteAllOnSubmit(client.PPVendeursClients);
                    ctxt.PPArticlesEnPaniers.DeleteAllOnSubmit(client.PPArticlesEnPaniers);
                    ctxt.PPClients.DeleteOnSubmit(client);
                }
                else
                {
                    client.Statut = (int)StatutCompte.DESACTIVE;
                    ctxt.PPCommandes.DeleteAllOnSubmit(client.PPCommandes);
                    foreach (var comm in client.PPCommandes)
                    {
                        ctxt.PPDetailsCommandes.DeleteAllOnSubmit(comm.PPDetailsCommandes);
                    }
                    ctxt.PPVendeursClients.DeleteAllOnSubmit(client.PPVendeursClients);
                }
            }
            
            ctxt.SubmitChanges();
        }

        private void SupprimerVendeurs(long[] ids)
        {
            var vendeursADelete =
                from vendeur
                    in ctxt.PPVendeurs
                where ids.Contains(vendeur.NoVendeur)
                select vendeur;

            foreach (var vendeur in vendeursADelete)
            {
                vendeur.Statut = (int)StatutCompte.DESACTIVE;
                foreach (var prod in vendeur.PPProduits)
                {
                    if (!prod.PPDetailsCommandes.Any())
                    {
                        ctxt.PPArticlesEnPaniers.DeleteAllOnSubmit(prod.PPArticlesEnPaniers);
                        ctxt.PPProduits.DeleteOnSubmit(prod);
                    }
                    else
                    {
                        prod.Disponibilité = null;
                    }
                }
            }
            
            ctxt.SubmitChanges();
        }

        private List<PPClient> ClientsInactifsPour(TimeSpan periode)
        {
            return
                (from client
                        in ctxt.PPClients
                        where client.Statut == (int)StatutCompte.ACTIF
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
                        where vendeur.Statut == (int)StatutCompte.ACTIF
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
        public ActionResult RefreshInactifs([FromUri]string typeUtilisateur, string recherche)
        {            
            if (typeUtilisateur == RolesUtil.UTIL)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisablePersonne", Tuple.Create(RechercheUtilisateurs(typeUtilisateur), true));
            } 
            if (typeUtilisateur == RolesUtil.VEND)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisableVendeur", Tuple.Create(RechercheUtilisateurs(typeUtilisateur).Cast<PPVendeur>().ToList(), true));
            }
            if (typeUtilisateur == RolesUtil.CLIENT)
            {
                return PartialView("Gestionnaire/Inactivite/_ListeDisableClient", Tuple.Create(RechercheUtilisateurs(typeUtilisateur).Cast<PPClient>().ToList(), true)); 
            }
            
            return HttpBadRequest();
        }
        
        [System.Web.Mvc.Route("Inactivite/Vendeurs")]
        public ActionResult RefreshInactifsVendeur(string depuisVendeur  = "T")
        {
            TimeSpan periodeVendeur = ParsePeriode(depuisVendeur);

            var viewModel = VendeursInactifsPour(periodeVendeur);

            return PartialView("Gestionnaire/Inactivite/_ListeSuppressionVendeur", viewModel);
        }

        [System.Web.Mvc.Route("Inactivite/Clients")]
        public ActionResult RefreshInactifsClient(string depuisClient  = "T")
        {
            TimeSpan periodeClient = ParsePeriode(depuisClient);

            var viewModel = ClientsInactifsPour(periodeClient);

            return PartialView("Gestionnaire/Inactivite/_ListeSuppressionClient", viewModel);
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
                    where util.Statut == (int)StatutCompte.ACTIF
                where (util.Nom + util.Prenom).Contains(nom)
                select util).Cast<IUtilisateur>().ToList();
        }

        private List<IUtilisateur> GetVendeursAvecNom(string nom)
        {
            return
                (from util
                    in ctxt.PPVendeurs
                where util.Statut == (int)StatutCompte.ACTIF && (util.Nom + util.Prenom).Contains(nom)
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
        #endregion

        #region Statistiques
        [System.Web.Mvc.Route("Statistiques")]
        public ActionResult Statistiques()
        {
            
            return View();
        }
        
        [System.Web.Mvc.Route("Statistiques/Donnees")]
        public ActionResult DonneesStats()
        {
            var clients = GetDonneesClients();
            var vendeurs = GetDonneesVendeurs();

            PageStatsViewModel data = new PageStatsViewModel
            {
                Clients = clients,
                Vendeurs = vendeurs
            };
            
            JsonSerializerSettings jss = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject(data, Formatting.Indented, jss);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<SerializablePPClient> GetDonneesClients()
        {
            return
                (from cli
                    in ctxt.PPClients
                select cli).ToList().Select(cli =>
                {
                    
                    return new SerializablePPClient(cli)
                    {
                        TotalCommandes = cli.PPCommandes
                            .Sum(c => c.MontantTotAvantTaxes ?? 0 + c.CoutLivraison ?? 0 + c.TPS ?? 0 + c.TVQ ?? 0),
                        TotalCommandesParVendeur = cli.PPCommandes
                            .GroupBy(c => c.PPVendeur)
                            .Select(g => new
                            {
                                g.Key,
                                AvantTaxes = g.Sum(c => c.MontantTotAvantTaxes.GetValueOrDefault()),
                                Livraison = g.Sum(c => c.CoutLivraison.GetValueOrDefault()),
                                Taxes = g.Sum(c => c.TPS.GetValueOrDefault() + c.TVQ.GetValueOrDefault()),
                                DerniereCommande = g.Max(c => c.DateCommande.GetValueOrDefault(DateTime.MinValue))
                            })
                            .ToDictionary(o => o.Key.NoVendeur,
                                o => new StatsVendeurClient
                                {
                                    Vendeur = new SerializablePPVendeur(o.Key),
                                    DateDerniereCommande = o.DerniereCommande,
                                    TotalCommandes = o.AvantTaxes + o.Livraison + o.Taxes,
                                    TotalTaxesEtLivraison = o.Livraison + o.Taxes,
                                    TotalBrut = o.AvantTaxes,
                                }
                            )
                    };
                }).ToList();
        }

        private List<SerializablePPVendeur> GetDonneesVendeurs()
        {
            return
                (from ven
                    in ctxt.PPVendeurs
                select ven).ToList()
                .Select(ven =>
                    new SerializablePPVendeur(ven)
                    {
                        typesClient = CalcNbClientsVendeur(ven),
                        TotalCommandes = ven.PPCommandes.Sum(c => c.MontantTotAvantTaxes.GetValueOrDefault(0))
                    }).ToList();
        }

        #if !DEBUG
            [Securise(RolesUtil.VEND, RolesUtil.ADMIN)]
        #endif
        [System.Web.Mvc.Route("Statistiques/NbClients/{id}")]
        public ActionResult GetNbClientsVendeur(int id)
        {
            var vendeur = (from vend in ctxt.PPVendeurs where vend.NoVendeur == id select vend).First();

            return Json(CalcNbClientsVendeur(vendeur));
        }
        
        private Dictionary<string, int> CalcNbClientsVendeur(PPVendeur vendeur)
        {
            long id = vendeur.NoVendeur;
            
            var actifs = vendeur.PPVendeursClients
                .Select(vc => vc.PPClient)
                .Count(c => c.PPCommandes.Any(comm => comm.NoVendeur == id));

            var potentiels = vendeur.PPVendeursClients
                .Select(vc => vc.PPClient)
                .Count(c => c.PPCommandes.All(comm => comm.NoVendeur != id)
                            && c.PPArticlesEnPaniers.Any(art => art.NoVendeur == id));

            var visiteurs = vendeur.PPVendeursClients
                .Select(vc => vc.PPClient)
                .Count(c => c.PPCommandes.All(comm => comm.NoVendeur != id)
                            && c.PPArticlesEnPaniers.All(art => art.NoVendeur != id));

            return new Dictionary<string, int>
            {
                {"actifs", actifs},
                {"potentiels", potentiels},
                {"visiteurs", visiteurs},
            };
        }
        
        #region Graphiques
        [System.Web.Mvc.Route("Statistiques/TousVendeurs")]
        public ActionResult TotalVendeurs(string periode = "T")
        {
            TimeSpan tPeriode = GraphiqueParsePeriode(periode);
            
            var vendeurs =
                from vendeur
                    in ctxt.PPVendeurs
                where vendeur.Statut == (int)StatutCompte.ACTIF ||
                      vendeur.Statut == (int)StatutCompte.DESACTIVE && vendeur.DateMAJ < DateTime.Now - tPeriode
                select vendeur;
            
            return Json(ConvertirEnDictionnaire(
                    vendeurs,
                    v => v.DateCreation.GetValueOrDefault(BEGINNING),
                    tousVendeurs => tousVendeurs.Count(),
                    tPeriode,
                    new TimeSpan(tPeriode.Ticks / 20)
                    ), JsonRequestBehavior.AllowGet);
        }
        
        private TimeSpan GraphiqueParsePeriode(string periode) {
            if (periode == "T")
            {
                return DateTime.Now - BEGINNING;
            }
            
            return ParsePeriode(periode);
        }
        
        [System.Web.Mvc.Route("Statistiques/NouveauxVendeurs")]
        public ActionResult NouveauxVendeurs(string periode)
        {
            TimeSpan tPeriode = GraphiqueParsePeriode(periode);
            var increment = new TimeSpan(tPeriode.Ticks / 20);
            
            var demandesEnAttente =
                from vendeur
                    in ctxt.PPVendeurs
                where vendeur.Statut != (int)StatutCompte.DESACTIVE ||
                      vendeur.DateCreation < (DateTime.Now - tPeriode)
                select vendeur;
            
            var demandesAcceptees =
                from vendeur
                    in ctxt.PPVendeurs
                where vendeur.Statut == (int)StatutCompte.ACTIF ||
                      vendeur.DateMAJ < (DateTime.Now - tPeriode)
                select vendeur;
            
            var dictEnAttente = ConvertirEnDictionnaire(
                demandesEnAttente,
                v => v.DateCreation.GetValueOrDefault(BEGINNING),
                tousVendeurs => tousVendeurs.Count(),
                tPeriode,
                increment       
            );
            
            var dictEnAccepte = ConvertirEnDictionnaire(
                demandesAcceptees,
                v => v.DateMAJ.GetValueOrDefault(BEGINNING),
                tousVendeurs => tousVendeurs.Count(),
                tPeriode,
                increment
            );
            
            return Json(new []{dictEnAttente, dictEnAccepte}, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.Route("Statistiques/TousClients")]
        public ActionResult TotalClients()
        {
            var actifs =
                (from cli
                        in ctxt.PPClients
                    where cli.PPCommandes.Any()
                    select cli).Count();

            var potentiels =
                (from cli
                        in ctxt.PPClients
                    where !cli.PPCommandes.Any() && cli.PPArticlesEnPaniers.Any()
                    select cli).Count();

            var visiteurs =
                (from cli
                        in ctxt.PPClients
                    where !cli.PPCommandes.Any() && !cli.PPArticlesEnPaniers.Any()
                    select cli).Count();


            return Json(
                Tuple.Create(
                    actifs,
                    potentiels,
                    visiteurs
                ), JsonRequestBehavior.AllowGet);
        }
        
        [System.Web.Mvc.Route("Statistiques/NouveauxClients")]
        public ActionResult NouveauxClients(string periode)
        {
            TimeSpan tPeriode = GraphiqueParsePeriode(periode);
            
            var clients =
                from cli
                    in ctxt.PPClients
                where cli.Statut == (int)StatutCompte.ACTIF &&
                      cli.DateCreation > DateTime.Now - tPeriode
                select cli;
            
            return Json(ConvertirEnDictionnaire(
                clients,
                c => c.DateCreation.GetValueOrDefault(BEGINNING),
                cs => cs.Count(),
                tPeriode,
                new TimeSpan(tPeriode.Ticks / 20)
            ), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion
        
        #region Redevances
        public ActionResult Redevances(int? id)
        {
            return id.HasValue ? Redevances(id.Value) : Redevances();
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
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("Redevances/Paiement/{idCommande}")]
        public ActionResult UpdateRedevancesVendeur([FromUri]int idCommande)
        {
            try
            {
                var commandeAUpdate =
                    (from commande
                            in ctxt.PPHistoriquePaiements
                        where commande.NoCommande == idCommande
                        select commande).First();

                commandeAUpdate.Redevance *= -1;
                
                ctxt.SubmitChanges();
                
                DetailsRedevances details = 
                    (from vendeur
                        in ctxt.PPVendeurs
                     join paiement
                         in ctxt.PPHistoriquePaiements
                         on vendeur.NoVendeur equals paiement.NoVendeur
                         into paiementsVendeur
                    where vendeur.NoVendeur == commandeAUpdate.NoVendeur
                    select new DetailsRedevances
                    {
                        Paiements = paiementsVendeur.ToList(),
                        Vendeur = vendeur,
                    }).First();
                
                return PartialView("Gestionnaire/Redevances/_DetailsRedevances", details);
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
        #endregion
    }
}