using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using PetitesPuces.Models;
using PetitesPuces.Securite;

namespace PetitesPuces.Controllers
{
    [Securise(RolesUtil.ADMIN)]
    public class JeuDessaiController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        private CultureInfo culture = new CultureInfo("en-US");
        // GET: JeuDessai

        public ActionResult Index(string status = "vider")
        {
            var PPArticlesEnPanier = (from val in context.PPArticlesEnPaniers
                select val).Count();
            var PPPoidsLivraisons = (from val in context.PPPoidsLivraisons
                select val).Count();
            var PPDetailsCommandes = (from val in context.PPDetailsCommandes
                select val).Count();
            var PPEvaluations = (from val in context.PPEvaluations
                select val).Count();
            var PPTypesPoids = (from val in context.PPTypesPoids
                select val).Count();
            var PPTypesLivraison = (from val in context.PPTypesLivraisons
                select val).Count();
            var PPCommandes = (from val in context.PPCommandes
                select val).Count();
            var PPTaxeFederale = (from val in context.PPTaxeFederales
                select val).Count();
            var PPTaxeProvinciale = (from val in context.PPTaxeProvinciales
                select val).Count();
            var PPProduits = (from val in context.PPProduits
                select val).Count();
            var PPClients = (from val in context.PPClients
                select val).Count();
            var PPVendeurs = (from val in context.PPVendeurs
                select val).Count();
            var PPGestionnaires = (from val in context.PPGestionnaires
                select val).Count();
            var PPVendeursClients = (from val in context.PPVendeursClients
                select val).Count();
            var PPDestinataires = (from val in context.PPDestinataires
                select val).Count();
            var PPMessages = (from val in context.PPMessages
                select val).Count();
            var PPLieu = (from val in context.PPLieus
                select val).Count();
            var PPCategories = (from val in context.PPCategories
                select val).Count();
            var PPHistoriquePaiements = (from val in context.PPHistoriquePaiements
                select val).Count();

            int[] intNbEnregistrement;
            intNbEnregistrement = new int[]
                {
                    PPCategories, PPLieu, PPMessages, PPDestinataires, PPGestionnaires, PPVendeurs,
                    PPProduits, PPClients, PPVendeursClients, PPHistoriquePaiements, PPTaxeProvinciale,
                    PPTaxeFederale, PPTypesLivraison, PPCommandes, PPTypesPoids, PPPoidsLivraisons,
                    PPArticlesEnPanier, PPDetailsCommandes, PPEvaluations
                }
                ;
            string[] strNomTable =
            {
                "PPCategories", "PPLieu", "PPMessages", "PPDestinataires", "PPGestionnaires", "PPVendeurs",
                "PPProduits", "PPClients",
                "PPVendeursClients", "PPHistoriquePaiements", "PPTaxeProvinciale", "PPTaxeFederale", "PPTypesLivraison",
                "PPCommandes", "PPTypesPoids", "PPPoidsLivraisons",
                "PPArticlesEnPanier", "PPDetailsCommandes", "PPEvaluations"
            };

            ViewBag.nomTable = strNomTable;
            ViewBag.nbEnregistrement = intNbEnregistrement;

/*
            string lsMessage = "";
            lsMessage = "Dans la table PPCategories, il y a : [ " + PPCategories + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPLieu, il y a : [ " + PPLieu + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPMessages, il y a : [ " + PPMessages + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPDestinataires, il y a : [ " + PPDestinataires + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPGestionnaires, il y a : [ " + PPGestionnaires + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPVendeurs, il y a : [ " + PPVendeurs + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPProduits, il y a : [ " + PPProduits + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPClients, il y a : [ " + PPClients + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPVendeursClients, il y a : [ " + PPVendeursClients + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPTaxeProvinciale, il y a : [ " + PPTaxeProvinciale + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPTaxeProvinciale, il y a : [ " + PPTaxeProvinciale + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPCommandes, il y a : [ " + PPCommandes + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPTypesLivraison, il y a : [ " + PPTypesLivraison + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPTypesPoids, il y a : [ " + PPTypesPoids + " ] enregistrement(s)" + "<br/>\n" +
                        "Dans la table PPPoidsLivraisons, il y a : [ " + PPPoidsLivraisons + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPArticlesEnPanier, il y a : [ " + PPArticlesEnPanier + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPDetailsCommandes, il y a : [ " + PPDetailsCommandes + " ] enregistrement(s)" +
                        "<br/>\n" +
                        "Dans la table PPEvaluations, il y a : [ " + PPEvaluations + " ] enregistrement(s)";
            ;*/
            //  ViewBag.lsMessageAfficher = lsMessage;

            ViewBag.statusVider = status;
            return View();
        }

        public ActionResult effacerBaseDonnee()
        {
            try
            {
                using (SqlConnection connection =
                    new SqlConnection(
                        WebConfigurationManager.ConnectionStrings["BD6B8_424QConnectionString"].ToString()))
                {
                    connection.Open();
                    SqlCommand delete = new SqlCommand
                    {
                        CommandText = "delete from PPEvaluations where 1=1" +
                                      "delete from PPDetailsCommandes where 1=1" +
                                      "delete from PPArticlesEnPanier where 1=1" +
                                      "delete from PPPoidsLivraisons where 1=1" +
                                      "delete from PPTypesPoids where 1=1" +
                                      "delete from PPCommandes where 1=1" +
                                      "delete from PPTypesLivraison where 1=1" +
                                      "delete from PPTaxeFederale where 1=1" +
                                      "delete from PPTaxeProvinciale where 1=1" +
                                      "delete from PPHistoriquePaiements where 1=1" +
                                      "delete from PPVendeursClients where 1=1" +
                                      "delete from PPClients where 1=1" +
                                      "delete from PPProduits where 1=1" +
                                      "delete from PPVendeurs where 1=1" +
                                      "delete from PPGestionnaires where 1=1" +
                                      "delete from PPDestinataires where 1=1" +
                                      "delete from PPMessages where 1=1" +
                                      "delete from PPLieu where 1=1" +
                                      "delete from PPCategories where 1=1",

                        Connection = connection
                    };
                    delete.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception e)
            {
                ViewBag.lsMessageSupprime = e.Message;
            }


            return RedirectToAction("Index", new {status = "remplir"});
        }

        public ActionResult ajouterLesDonnees()
        {
            try
            {
                /*
                 * PPTypesPoids
                 */
                var docTypePoids = (from typePoids in XDocument
                        .Load(Server.MapPath("/XML/PPTypesPoids.xml")).Descendants("row")
                    select new
                    {
                        CodePoids = typePoids.Element("CodePoids").Value,
                        PoidsMin = typePoids.Element("PoidsMin").Value,
                        PoidsMax = typePoids.Element("PoidsMax").Value
                    }).ToList();

                PPTypesPoid typesPoid;

                foreach (var type in docTypePoids)
                {
                    typesPoid = new PPTypesPoid();


                    typesPoid.CodePoids = Convert.ToInt16(type.CodePoids);
                    typesPoid.PoidsMin = Convert.ToDecimal(type.PoidsMin, culture);
                    typesPoid.PoidsMax = Convert.ToDecimal(type.PoidsMax, culture);

                    context.PPTypesPoids.InsertOnSubmit(typesPoid);
                }

                /*
                 * PPTypesLivraison
                 */
                var docTypeLivraison = (from typeLivraison in XDocument
                        .Load(Server.MapPath("/XML/PPTypesLivraison.xml")).Descendants("row")
                    select new
                    {
                        CodeLivraison = typeLivraison.Element("CodeLivraison").Value,
                        Description = typeLivraison.Element("Description").Value
                    }).ToList();
                PPTypesLivraison objTypesLivraison;

                foreach (var type in docTypeLivraison)
                {
                    objTypesLivraison = new PPTypesLivraison();

                    objTypesLivraison.CodeLivraison = Convert.ToInt16(type.CodeLivraison);
                    objTypesLivraison.Description = type.Description;

                    context.PPTypesLivraisons.InsertOnSubmit(objTypesLivraison);
                }

                /*
                 * PPPoidsLivraison
                 */
                var docPoidsLivraison = (from poidslivraison in XDocument
                        .Load(Server.MapPath("/XML/PPPoidsLivraison.xml")).Descendants("row")
                    select new
                    {
                        CodeLivraison = poidslivraison.Element("CodeLivraison").Value,
                        CodePoids = poidslivraison.Element("CodePoids").Value,
                        Tarif = poidslivraison.Element("Tarif").Value
                    }).ToList();

                PPPoidsLivraison podLivraison;

                foreach (var type in docPoidsLivraison)
                {
                    podLivraison = new PPPoidsLivraison();

                    podLivraison.CodeLivraison = Convert.ToInt16(type.CodeLivraison);
                    podLivraison.CodePoids = Convert.ToInt16(type.CodePoids);
                    podLivraison.Tarif = Convert.ToDecimal(type.Tarif, culture);


                    context.PPPoidsLivraisons.InsertOnSubmit(podLivraison);
                }

                /*
                 * PPCategories
                 */
                var docCategorie = (from unCategorie in XDocument
                        .Load(Server.MapPath("/XML/PPCategorie.xml")).Descendants("row")
                    select new
                    {
                        NoCategorie = unCategorie.Element("NoCategorie").Value,
                        Description = unCategorie.Element("Description").Value,
                        Details = unCategorie.Element("Details").Value
                    }).ToList();

                PPCategory category;

                foreach (var unCat in docCategorie)
                {
                    category = new PPCategory();

                    category.NoCategorie = Convert.ToInt32(unCat.NoCategorie);
                    category.Details = unCat.Details;
                    category.Description = unCat.Description;

                    context.PPCategories.InsertOnSubmit(category);
                }

                /*
                 * PPGestionnaires
                 */
                var objGestionnaire = (from unGestionnaire in XDocument
                        .Load(Server.MapPath("/XML/PPGestionnaire.xml")).Descendants("row")
                    select new PPGestionnaire
                    {
                        NoGestionnaire = Convert.ToInt64(unGestionnaire.Element("NoGestionnaire").Value),
                        Nom = unGestionnaire.Element("Nom").Value,
                        Prenom = unGestionnaire.Element("Prenom").Value,
                        AdresseEmail = unGestionnaire.Element("AdresseEmail").Value,
                        MotDePasse = unGestionnaire.Element("MotDePasse").Value
                    }).ToList();

                foreach (var objG in objGestionnaire)
                    using (SqlConnection connection =
                        new SqlConnection(
                            WebConfigurationManager.ConnectionStrings["BD6B8_424QConnectionString"].ToString()))
                    {
                        connection.Open();
                        SqlCommand insert = new SqlCommand
                        {
                            CommandText = string.Format(
                                "INSERT INTO PPGestionnaires (NoGestionnaire, Nom, Prenom, AdresseEmail, MotDePasse) VALUES (" +
                                "{0}, '{1}', '{2}', '{3}', '{4}')", objG.NoGestionnaire, objG.Nom, objG.Prenom,
                                objG.AdresseEmail, objG.MotDePasse),
                            Connection = connection
                        };
                        insert.ExecuteNonQuery();
                        connection.Close();
                    }


                /*
                 * PPClients
                 */
                var docClient = (from unClient in XDocument
                        .Load(Server.MapPath("/XML/PPClients.xml")).Descendants("row")
                    select new
                    {
                        NoClient = unClient.Element("NoClient").Value,
                        AdresseEmail = unClient.Element("AdresseEmail").Value,
                        MotDePasse = unClient.Element("MotDePasse").Value,
                        Nom = unClient.Element("Nom").Value,
                        Prenom = unClient.Element("Prenom").Value,
                        Rue = unClient.Element("Rue").Value,
                        Ville = unClient.Element("Ville").Value,
                        Province = unClient.Element("Province").Value,
                        CodePostal = unClient.Element("CodePostal").Value,
                        Pays = unClient.Element("Pays").Value,
                        Tel1 = unClient.Element("Tel1").Value,
                        Tel2 = unClient.Element("Tel2").Value,
                        DateCreation = unClient.Element("DateCreation").Value,
                        DateMAJ = unClient.Element("DateMAJ").Value,
                        NbConnexions = unClient.Element("NbConnexions").Value,
                        DateDerniereConnexion = unClient.Element("DateDerniereConnexion").Value,
                        Statut = unClient.Element("Statut").Value
                    }).ToList();

                PPClient objClient;

                foreach (var unclient in docClient)
                {
                    objClient = new PPClient();

                    objClient.NoClient = Convert.ToInt64(unclient.NoClient);
                    objClient.AdresseEmail = unclient.AdresseEmail;
                    objClient.MotDePasse = unclient.MotDePasse;
                    objClient.Nom = unclient.Nom;
                    objClient.Prenom = unclient.Prenom;
                    objClient.Rue = unclient.Rue;
                    objClient.Ville = unclient.Ville;
                    objClient.Province = unclient.Province;
                    objClient.CodePostal = unclient.CodePostal;
                    objClient.Pays = unclient.Pays;
                    objClient.Tel1 = unclient.Tel1;
                    objClient.Tel2 = unclient.Tel2;
                    if (unclient.DateCreation != "") objClient.DateCreation = Convert.ToDateTime(unclient.DateCreation);
                    if (unclient.DateMAJ != "") objClient.DateMAJ = Convert.ToDateTime(unclient.DateMAJ);
                    if (unclient.DateDerniereConnexion != "")
                        objClient.DateDerniereConnexion = Convert.ToDateTime(unclient.DateDerniereConnexion);
                    objClient.NbConnexions = Convert.ToInt16(unclient.NbConnexions);
                    objClient.Statut = Convert.ToInt16(unclient.Statut);

                    context.PPClients.InsertOnSubmit(objClient);
                }

                /*
                 * PPVendeurs
                 */
                var docVendeur = (from unVendeur in XDocument
                        .Load(Server.MapPath("/XML/PPVendeur.xml")).Descendants("row")
                    select new
                    {
                        NoVendeur = unVendeur.Element("NoVendeur").Value,
                        NomAffaires = unVendeur.Element("NomAffaires").Value,
                        Nom = unVendeur.Element("Nom").Value,
                        Prenom = unVendeur.Element("Prenom").Value,
                        Rue = unVendeur.Element("Rue").Value,
                        Ville = unVendeur.Element("Ville").Value,
                        Province = unVendeur.Element("Province").Value,
                        CodePostal = unVendeur.Element("CodePostal").Value,
                        Pays = unVendeur.Element("Pays").Value,
                        Tel1 = unVendeur.Element("Tel1").Value,
                        Tel2 = unVendeur.Element("Tel2").Value,
                        AdresseEmail = unVendeur.Element("AdresseEmail").Value,
                        MotDePasse = unVendeur.Element("MotDePasse").Value,
                        PoidsMaxLivraison = unVendeur.Element("PoidsMaxLivraison").Value,
                        LivraisonGraduite = unVendeur.Element("LivraisonGraduite").Value,
                        Taxes = unVendeur.Element("Taxes").Value,
                        Pourcentage = unVendeur.Element("Pourcentage").Value,
                        Configuration = unVendeur.Element("Configuration").Value,
                        DateCreation = unVendeur.Element("DateCreation").Value,
                        DateMAJ = unVendeur.Element("DateMAJ").Value,
                        Statut = unVendeur.Element("Statut").Value
                    }).ToList();

                PPVendeur objVendeur;

                foreach (var unVendeur in docVendeur)
                {
                    objVendeur = new PPVendeur();

                    objVendeur.NoVendeur = Convert.ToInt64(unVendeur.NoVendeur);

                    objVendeur.NomAffaires = unVendeur.NomAffaires;
                    objVendeur.Nom = unVendeur.Nom;
                    objVendeur.Prenom = unVendeur.Prenom;
                    objVendeur.Rue = unVendeur.Rue;
                    objVendeur.Ville = unVendeur.Ville;
                    objVendeur.Province = unVendeur.Province;
                    objVendeur.CodePostal = unVendeur.CodePostal;
                    objVendeur.Pays = unVendeur.Pays;
                    objVendeur.Tel1 = unVendeur.Tel1;
                    objVendeur.Tel2 = unVendeur.Tel2;
                    objVendeur.AdresseEmail = unVendeur.AdresseEmail;
                    objVendeur.MotDePasse = unVendeur.MotDePasse;
                    objVendeur.PoidsMaxLivraison = Convert.ToInt32(unVendeur.PoidsMaxLivraison);
                    objVendeur.LivraisonGratuite = Convert.ToDecimal(unVendeur.LivraisonGraduite, culture);
                    objVendeur.Taxes = unVendeur.Taxes == "1" ? true : false;
                    objVendeur.Pourcentage = Convert.ToDecimal(unVendeur.Pourcentage, culture);
                    if (unVendeur.DateCreation != "")
                        objVendeur.DateCreation = Convert.ToDateTime(unVendeur.DateCreation);
                    if (unVendeur.DateMAJ != "") objVendeur.DateMAJ = Convert.ToDateTime(unVendeur.DateMAJ);
                    objVendeur.Configuration = unVendeur.Configuration;
                    objVendeur.Statut = Convert.ToInt16(unVendeur.Statut);

                    context.PPVendeurs.InsertOnSubmit(objVendeur);
                }

                /*
                 * PPVendeursClients
                 */
                var docVendeurClient = (from unVendeurClient in XDocument
                        .Load(Server.MapPath("/XML/PPVendeurClient.xml")).Descendants("row")
                    select new
                    {
                        NoVendeur = unVendeurClient.Element("NoVendeur").Value,
                        NoClient = unVendeurClient.Element("NoClient").Value,
                        DateVisite = unVendeurClient.Element("DateVisite").Value
                    }).ToList();

                PPVendeursClient objVendeursClient;

                foreach (var unvendeursClient in docVendeurClient)
                {
                    objVendeursClient = new PPVendeursClient();

                    objVendeursClient.NoVendeur = Convert.ToInt64(unvendeursClient.NoVendeur);
                    objVendeursClient.NoClient = Convert.ToInt64(unvendeursClient.NoClient);
                    if (unvendeursClient.DateVisite != "")
                        objVendeursClient.DateVisite = Convert.ToDateTime(unvendeursClient.DateVisite);

                    context.PPVendeursClients.InsertOnSubmit(objVendeursClient);
                }

                /*
                 * PPTaxeFederale
                 */
                var docTPS = (from taxFederal in XDocument
                        .Load(Server.MapPath("/XML/PPTaxeFederale.xml")).Descendants("row")
                    select new
                    {
                        NoTPS = taxFederal.Element("NoTPS").Value,
                        DateEffectiveTPS = taxFederal.Element("DateEffectiveTPS").Value,
                        TauxTPS = taxFederal.Element("TauxTPS").Value
                    }).ToList();

                PPTaxeFederale objTaxeFederale;

                foreach (var taxe in docTPS)
                {
                    objTaxeFederale = new PPTaxeFederale();

                    objTaxeFederale.NoTPS = Convert.ToByte(taxe.NoTPS);
                    objTaxeFederale.DateEffectiveTPS = Convert.ToDateTime(taxe.DateEffectiveTPS);
                    objTaxeFederale.TauxTPS = Convert.ToDecimal(taxe.TauxTPS, culture);

                    context.PPTaxeFederales.InsertOnSubmit(objTaxeFederale);
                }

                /*
                 * PPTaxeProvinciale
                 */
                var docTVQ = (from taxeTVQ in XDocument
                        .Load(Server.MapPath("/XML/PPTaxeProvinciale.xml")).Descendants("row")
                    select new
                    {
                        NoTVQ = taxeTVQ.Element("NoTVQ").Value,
                        DateEffectiveTVQ = taxeTVQ.Element("DateEffectiveTVQ").Value,
                        TauxTVQ = taxeTVQ.Element("TauxTVQ").Value
                    }).ToList();

                PPTaxeProvinciale objTaxeProvinciale;

                foreach (var taxe in docTVQ)
                {
                    objTaxeProvinciale = new PPTaxeProvinciale();

                    objTaxeProvinciale.NoTVQ = Convert.ToByte(taxe.NoTVQ);
                    objTaxeProvinciale.DateEffectiveTVQ = Convert.ToDateTime(taxe.DateEffectiveTVQ);
                    objTaxeProvinciale.TauxTVQ = Convert.ToDecimal(taxe.TauxTVQ, culture);

                    context.PPTaxeProvinciales.InsertOnSubmit(objTaxeProvinciale);
                }


                /*
                 * PPProduits
                 */
                var docProduits = (from unProduit in XDocument
                        .Load(Server.MapPath("/XML/PPProduits.xml")).Descendants("row")
                    select new
                    {
                        NoProduit = unProduit.Element("NoProduit").Value,
                        NoVendeur = unProduit.Element("NoVendeur").Value,
                        NoCategorie = unProduit.Element("NoCategorie").Value,
                        Nom = unProduit.Element("Nom").Value,
                        Description = unProduit.Element("Description").Value,
                        Photo = unProduit.Element("Photo").Value,
                        PrixDemande = unProduit.Element("PrixDemande").Value,
                        NombreItems = unProduit.Element("NombreItems").Value,
                        Disponibilite = unProduit.Element("Disponibilite").Value,
                        DateVente = unProduit.Element("DateVente").Value,
                        PrixVente = unProduit.Element("PrixVente").Value,
                        Poids = unProduit.Element("Poids").Value,
                        DateCreation = unProduit.Element("DateCreation").Value,
                        DateMAJ = unProduit.Element("DateMAJ").Value
                    }).ToList();
                PPProduit objProduit;

                foreach (var type in docProduits)
                {
                    objProduit = new PPProduit();

                    objProduit.NoProduit = Convert.ToInt64(type.NoProduit);
                    objProduit.NoVendeur = Convert.ToInt64(type.NoVendeur);
                    objProduit.NoCategorie = Convert.ToInt32(type.NoCategorie);
                    objProduit.Nom = type.Nom;
                    objProduit.Description = type.Description;
                    objProduit.Photo = type.Photo;
                    objProduit.Disponibilité = type.Disponibilite != "1" ? false : true;
                    objProduit.PrixDemande = Convert.ToDecimal(type.PrixDemande, culture);
                    objProduit.NombreItems = Convert.ToInt16(type.NombreItems);
                    if (type.DateVente != "") objProduit.DateVente = Convert.ToDateTime(type.DateVente, culture);
                    if (type.PrixVente != "") objProduit.PrixVente = Convert.ToDecimal(type.PrixVente, culture);
                    objProduit.Poids = Convert.ToDecimal(type.Poids, culture);
                    objProduit.DateCreation = Convert.ToDateTime(type.DateCreation);
                    if (type.DateMAJ != "") objProduit.DateMAJ = Convert.ToDateTime(type.DateMAJ);

                    context.PPProduits.InsertOnSubmit(objProduit);
                }

                /*
                 * PPArticlesEnPanier
                 */
                var docArticleEnPanier = (from unPanier in XDocument
                        .Load(Server.MapPath("/XML/PPArticlesEnPanier.xml")).Descendants("row")
                    select new
                    {
                        NoPanier = unPanier.Element("NoPanier").Value,
                        NoClient = unPanier.Element("NoClient").Value,
                        NoVendeur = unPanier.Element("NoVendeur").Value,
                        NoProduit = unPanier.Element("NoProduit").Value,
                        DateCreation = unPanier.Element("DateCreation").Value,
                        NbItems = unPanier.Element("NbItems").Value
                    }).ToList();

                PPArticlesEnPanier objPanier;

                foreach (var type in docArticleEnPanier)
                {
                    objPanier = new PPArticlesEnPanier();

                    objPanier.NoPanier = Convert.ToInt64(type.NoPanier);
                    objPanier.NoClient = Convert.ToInt64(type.NoClient);
                    objPanier.NoVendeur = Convert.ToInt64(type.NoVendeur);
                    objPanier.NoProduit = Convert.ToInt64(type.NoProduit);
                    objPanier.DateCreation = Convert.ToDateTime(type.DateCreation);
                    objPanier.NbItems = Convert.ToInt16(type.NbItems);

                    context.PPArticlesEnPaniers.InsertOnSubmit(objPanier);
                }

                /*
                 * PPCommandes
                 */
                var docCommande = (from commande in XDocument
                        .Load(Server.MapPath("/XML/PPCommandes.xml")).Descendants("row")
                    select new
                    {
                        NoCommande = commande.Element("NoCommande").Value,
                        NoClient = commande.Element("NoClient").Value,
                        NoVendeur = commande.Element("NoVendeur").Value,
                        DateCommande = commande.Element("DateCommande").Value,
                        CoutLivraison = commande.Element("CoutLivraison").Value,
                        TypeLivraison = commande.Element("TypeLivraison").Value,
                        MontantTotAvantTaxes = commande.Element("MontantTotAvantTaxes").Value,
                        TPS = commande.Element("TPS").Value,
                        TVQ = commande.Element("TVQ").Value,
                        PoidsTotal = commande.Element("PoidsTotal").Value,
                        Statut = commande.Element("Statut").Value,
                        NoAutorisation = commande.Element("NoAutorisation").Value
                    }).ToList();

                PPCommande objcommande;

                foreach (var type in docCommande)
                {
                    objcommande = new PPCommande();

                    objcommande.NoCommande = Convert.ToInt64(type.NoCommande);
                    objcommande.NoClient = Convert.ToInt64(type.NoClient);
                    objcommande.NoVendeur = Convert.ToInt64(type.NoVendeur);
                    objcommande.DateCommande = Convert.ToDateTime(type.DateCommande);
                    if (type.CoutLivraison != "")
                        objcommande.CoutLivraison = Convert.ToDecimal(type.CoutLivraison, culture);
                    objcommande.TypeLivraison = Convert.ToInt16(type.TypeLivraison);
                    objcommande.MontantTotAvantTaxes = Convert.ToDecimal(type.MontantTotAvantTaxes, culture);
                    if (type.TPS != "") objcommande.TPS = Convert.ToDecimal(type.TPS, culture);
                    if (type.TVQ != "") objcommande.TVQ = Convert.ToDecimal(type.TVQ, culture);
                    objcommande.PoidsTotal = Convert.ToDecimal(type.PoidsTotal, culture);
                    objcommande.Statut = Convert.ToChar(type.Statut);
                    objcommande.NoAutorisation = type.NoAutorisation;

                    context.PPCommandes.InsertOnSubmit(objcommande);
                }


                /*
                 * PPDetailsCommande
                 */
                var docDetailsCommande = (from unDetailCommande in XDocument
                        .Load(Server.MapPath("/XML/PPDetailsCommande.xml")).Descendants("row")
                    select new
                    {
                        NoDetailCommandes = unDetailCommande.Element("NoDetailCommandes").Value,
                        NoCommande = unDetailCommande.Element("NoCommande").Value,
                        NoProduit = unDetailCommande.Element("NoProduit").Value,
                        PrixVente = unDetailCommande.Element("PrixVente").Value,
                        Quantité = unDetailCommande.Element("Quantité").Value
                    }).ToList();

                PPDetailsCommande objDetailsCommande;

                foreach (var type in docDetailsCommande)
                {
                    objDetailsCommande = new PPDetailsCommande();

                    objDetailsCommande.NoDetailCommandes = Convert.ToInt64(type.NoDetailCommandes);
                    objDetailsCommande.NoCommande = Convert.ToInt64(type.NoCommande);
                    objDetailsCommande.NoProduit = Convert.ToInt64(type.NoProduit);
                    objDetailsCommande.PrixVente = Convert.ToDecimal(type.PrixVente, culture);
                    objDetailsCommande.Quantité = Convert.ToInt16(type.Quantité);

                    context.PPDetailsCommandes.InsertOnSubmit(objDetailsCommande);
                }

                /*
                 * PPHistoriquePayement
                 */
                var docHistorique = (from uneHistorique in XDocument
                        .Load(Server.MapPath("/XML/PPHistoriquePayement.xml"))
                        .Descendants("row")
                    select new
                    {
                        NoHistorique = uneHistorique.Element("NoHistorique").Value,
                        MontantVenteAvantLivraison = uneHistorique.Element("MontantVenteAvantLivraison").Value,
                        NoVendeur = uneHistorique.Element("NoVendeur").Value,
                        NoClient = uneHistorique.Element("NoClient").Value,
                        NoCommande = uneHistorique.Element("NoCommande").Value,
                        DateVente = uneHistorique.Element("DateVente").Value,
                        NoAutoristion = uneHistorique.Element("NoAutoristion").Value,
                        FraisLesi = uneHistorique.Element("FraisLesi").Value,
                        Redevance = uneHistorique.Element("Redevance").Value,
                        FraisLivraison = uneHistorique.Element("FraisLivraison").Value,
                        FraisTPS = uneHistorique.Element("FraisTPS").Value,
                        FraisTVQ = uneHistorique.Element("FraisTVQ").Value
                    }).ToList();

                PPHistoriquePaiement objHistoriquePaiement;

                foreach (var unHistorique in docHistorique)
                {
                    objHistoriquePaiement = new PPHistoriquePaiement();

                    objHistoriquePaiement.NoHistorique = Convert.ToInt32(unHistorique.NoHistorique);
                    objHistoriquePaiement.MontantVenteAvantLivraison =
                        Convert.ToDecimal(unHistorique.MontantVenteAvantLivraison, culture);
                    objHistoriquePaiement.NoVendeur = Convert.ToInt64(unHistorique.NoVendeur);
                    objHistoriquePaiement.NoClient = Convert.ToInt64(unHistorique.NoClient);
                    objHistoriquePaiement.NoCommande = Convert.ToInt64(unHistorique.NoCommande);
                    objHistoriquePaiement.DateVente = Convert.ToDateTime(unHistorique.DateVente);
                    objHistoriquePaiement.NoAutorisation = unHistorique.NoAutoristion;
                    objHistoriquePaiement.FraisLesi = Convert.ToDecimal(unHistorique.FraisLesi, culture);
                    objHistoriquePaiement.Redevance = Convert.ToDecimal(unHistorique.Redevance, culture);
                    if (unHistorique.FraisLivraison != "")
                        objHistoriquePaiement.FraisLivraison = Convert.ToDecimal(unHistorique.FraisLivraison, culture);
                    if (unHistorique.FraisTPS != "")
                        objHistoriquePaiement.FraisTPS = Convert.ToDecimal(unHistorique.FraisTPS, culture);
                    if (unHistorique.FraisTVQ != "")
                        objHistoriquePaiement.FraisTVQ = Convert.ToDecimal(unHistorique.FraisTVQ, culture);


                    context.PPHistoriquePaiements.InsertOnSubmit(objHistoriquePaiement);
                }

                /*
                 * PPLieu
                 */
                var docLieu = (from unLieu in XDocument.Load(Server.MapPath("/XML/PPLieu.xml"))
                        .Descendants("row")
                    select new
                    {
                        NoLieu = unLieu.Element("NoLieu").Value,
                        Description = unLieu.Element("Description").Value
                    }).ToList();

                PPLieu objLieu;

                foreach (var unLieu in docLieu)
                {
                    objLieu = new PPLieu();

                    objLieu.NoLieu = Convert.ToInt16(unLieu.NoLieu);
                    objLieu.Description = unLieu.Description;


                    context.PPLieus.InsertOnSubmit(objLieu);
                }


                /*
                 * PPDestinataires
                 */
                var docDestinataires = (from unDestinataire in XDocument
                        .Load(Server.MapPath("/XML/PPDestinataires.xml"))
                        .Descendants("row")
                    select new
                    {
                        NoMsg = unDestinataire.Element("NoMsg").Value,
                        NoDestinataire = unDestinataire.Element("NoDestinataire").Value,
                        EtatLu = unDestinataire.Element("EtatLu").Value,
                        Lieu = unDestinataire.Element("Lieu").Value
                    }).ToList();

                PPDestinataire ppDestinataire;

                foreach (var unDest in docDestinataires)
                {
                    ppDestinataire=new PPDestinataire();

                    ppDestinataire.NoMsg = Convert.ToInt32(unDest.NoMsg);
                    ppDestinataire.NoDestinataire = Convert.ToInt32(unDest.NoDestinataire);
                    ppDestinataire.EtatLu = Convert.ToInt16(unDest.EtatLu);
                    ppDestinataire.Lieu = Convert.ToInt16(unDest.Lieu);
                    


                    context.PPDestinataires.InsertOnSubmit(ppDestinataire);
                }

                /*
                * PPMessages
                */
                var docMessages = (from unMessage in XDocument
                        .Load(Server.MapPath("/XML/PPMessages.xml"))
                        .Descendants("row")
                    select new
                    {
                        NoMsg = unMessage.Element("NoMsg").Value,
                        NoExpediteur = unMessage.Element("NoExpediteur").Value,
                        DescMsg = unMessage.Element("DescMsg").Value,
                        FichierJoint = unMessage.Element("FichierJoint").Value,
                        Lieu = unMessage.Element("Lieu").Value,
                        dateEnvoi = unMessage.Element("dateEnvoi").Value,
                        objet = unMessage.Element("objet").Value
                    }).ToList();

                PPMessage objMessage;

                foreach (var unMessages in docMessages)
                {
                    objMessage = new PPMessage();

                    objMessage.NoMsg = Convert.ToInt32(unMessages.NoMsg);
                    objMessage.NoExpediteur = Convert.ToInt32(unMessages.NoExpediteur);
                    objMessage.DescMsg = unMessages.DescMsg;
                    objMessage.FichierJoint = unMessages.FichierJoint;
                    objMessage.Lieu = Convert.ToInt16(unMessages.Lieu);
                    objMessage.dateEnvoi = Convert.ToDateTime(unMessages.dateEnvoi);
                    objMessage.objet = unMessages.objet;

                    context.PPMessages.InsertOnSubmit(objMessage);
                }
                /*
              * PPEvaluation
              */
                var docEvalu = (from uneEvaluation in XDocument
                        .Load(Server.MapPath("/XML/PPEvaluations.xml"))
                        .Descendants("row")
                    select new
                    {
                        NoClient = uneEvaluation.Element("NoClient").Value,
                        NoProduit = uneEvaluation.Element("NoProduit").Value,
                        Cote = uneEvaluation.Element("Cote").Value,
                        Commentaire = uneEvaluation.Element("Commentaire").Value,
                        DateMAJ = uneEvaluation.Element("DateMAJ").Value,
                        DateCreation = uneEvaluation.Element("DateCreation").Value
                    }).ToList();

                PPEvaluation objEvaluation;

                foreach (var uneEvalu in docEvalu)
                {
                    objEvaluation = new PPEvaluation();

                    objEvaluation.NoClient = Convert.ToInt32(uneEvalu.NoClient);
                    objEvaluation.NoProduit = Convert.ToInt32(uneEvalu.NoProduit);
                    objEvaluation.Cote_ =Convert.ToDecimal( uneEvalu.Cote);
                    objEvaluation.Commentaire_ = uneEvalu.Commentaire;
                    objEvaluation.DateMAJ_ = Convert.ToDateTime(uneEvalu.DateMAJ);
                    objEvaluation.DateCreation_ = Convert.ToDateTime(uneEvalu.DateCreation);

                    context.PPEvaluations.InsertOnSubmit(objEvaluation);
                }
                try
                {
                    context.SubmitChanges();
                }
                catch (Exception e)
                {
                    ViewBag.lsMessageRemplir = e.Message;
                }


                return RedirectToAction("Index", new {status = "vider"});
            }
            catch (Exception e)
            {
                return Content("Remplissage échoué: " + e);
            }
        }
    }
}