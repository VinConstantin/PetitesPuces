using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class JeuDessaiController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        // GET: JeuDessai

        public ActionResult Index()
        {
            String test = "";
            /*
             * PPTypesPoids
             */
            var docTypePoids = (from typePoids in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPTypesPoids.xml").Descendants("row")
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
                typesPoid.PoidsMin = Convert.ToDecimal(type.PoidsMin);
                typesPoid.PoidsMax = Convert.ToDecimal(type.PoidsMax);

                context.PPTypesPoids.InsertOnSubmit(typesPoid);
            }

            /*
             * PPTypesLivraison
             */
            var docTypeLivraison = (from typeLivraison in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPTypesLivraison.xml").Descendants("row")
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
            var docPoidsLivraison = (from poidslivraison in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPPoidsLivraison.xml").Descendants("row")
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
                podLivraison.Tarif = Convert.ToDecimal(type.Tarif);
                test += "Code livraison du poids livraison" + type.CodeLivraison;

                context.PPPoidsLivraisons.InsertOnSubmit(podLivraison);
            }

            /*
             * PPCategories
             */
            var docCategorie = (from unCategorie in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPCategorie.xml").Descendants("row")
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
            var objG = (from unGestionnaire in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPGestionnaire.xml").Descendants("row")
                select new PPGestionnaire
                {
                    NoGestionnaire = Convert.ToInt64(unGestionnaire.Element("NoGestionnaire").Value),
                    Nom = unGestionnaire.Element("Nom").Value,
                    Prenom = unGestionnaire.Element("Prenom").Value,
                    AdresseEmail = unGestionnaire.Element("AdresseEmail").Value,
                    MotDePasse = unGestionnaire.Element("MotDePasse").Value
                }).First();

            using (SqlConnection connection =
                new SqlConnection(WebConfigurationManager.ConnectionStrings["BD6B8_424QConnectionString"].ToString()))
            {
                connection.Open();
                SqlCommand insert = new SqlCommand
                {
                    CommandText= string.Format("INSERT INTO PPGestionnaires (NoGestionnaire, Nom, Prenom, AdresseEmail, MotDePasse) VALUES (" +
                                 "{0}, '{1}', '{2}', '{3}', '{4}')", objG.NoGestionnaire, objG.Nom, objG.Prenom, objG.AdresseEmail, objG.MotDePasse),
                    Connection = connection
                };
                insert.ExecuteNonQuery();
                connection.Close();
            }



                /*
                 * PPClients
                 */
                var docClient = (from unClient in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPClients.xml").Descendants("row")
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
                if (unclient.DateDerniereConnexion != "") objClient.DateDerniereConnexion = Convert.ToDateTime(unclient.DateDerniereConnexion);
                objClient.NbConnexions = Convert.ToInt16(unclient.NbConnexions);
                objClient.Statut = Convert.ToInt16(unclient.Statut);

                context.PPClients.InsertOnSubmit(objClient);
            }

            /*
             * PPVendeurs
             */
            var docVendeur = (from unVendeur in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPVendeur.xml").Descendants("row")
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
                objVendeur.LivraisonGratuite = Convert.ToDecimal(unVendeur.LivraisonGraduite);
                objVendeur.Taxes = unVendeur.Taxes == "1" ? true : false;
                objVendeur.Pourcentage = Convert.ToDecimal(unVendeur.Pourcentage);
                if (unVendeur.DateCreation != "") objVendeur.DateCreation = Convert.ToDateTime(unVendeur.DateCreation);
                if (unVendeur.DateMAJ != "") objVendeur.DateMAJ = Convert.ToDateTime(unVendeur.DateMAJ);
                objVendeur.Configuration = unVendeur.Configuration;
                objVendeur.Statut = Convert.ToInt16(unVendeur.Statut);

                context.PPVendeurs.InsertOnSubmit(objVendeur);
            }

            /*
             * PPVendeursClients
             */
            var docVendeurClient = (from unVendeurClient in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPVendeurClient.xml").Descendants("row")
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
                if (unvendeursClient.DateVisite != "") objVendeursClient.DateVisite = Convert.ToDateTime(unvendeursClient.DateVisite);
               
                context.PPVendeursClients.InsertOnSubmit(objVendeursClient);

            }

            /*
             * PPTaxeFederale
             */
            var docTPS = (from taxFederal in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPTaxeFederale.xml").Descendants("row")
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
                objTaxeFederale.TauxTPS = Convert.ToDecimal(taxe.TauxTPS);

                context.PPTaxeFederales.InsertOnSubmit(objTaxeFederale);
            }

            /*
             * PPTaxeProvinciale
             */
            var docTVQ = (from taxeTVQ in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPTaxeProvinciale.xml").Descendants("row")
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
                objTaxeProvinciale.TauxTVQ = Convert.ToDecimal(taxe.TauxTVQ);

                context.PPTaxeProvinciales.InsertOnSubmit(objTaxeProvinciale);
            }

           

            /*
             * PPProduits
             */
            var docProduits = (from unProduit in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPProduits.xml").Descendants("row")
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
                objProduit.PrixDemande = Convert.ToDecimal(type.PrixDemande);
                objProduit.NombreItems = Convert.ToInt16(type.NombreItems);
                if (type.DateVente != "") objProduit.DateVente = Convert.ToDateTime(type.DateVente);
                if (type.PrixVente != "") objProduit.PrixVente = Convert.ToDecimal(type.PrixVente);
                objProduit.Poids = Convert.ToDecimal(type.Poids);
                objProduit.DateCreation = Convert.ToDateTime(type.DateCreation);
                if (type.DateMAJ != "") objProduit.DateMAJ = Convert.ToDateTime(type.DateMAJ);

                context.PPProduits.InsertOnSubmit(objProduit);
            }

            /*
             * PPArticlesEnPanier
             */
            var docArticleEnPanier = (from unPanier in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPArticlesEnPanier.xml").Descendants("row")
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
            var docCommande = (from commande in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPCommandes.xml").Descendants("row")
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
                    if (type.CoutLivraison != "") objcommande.CoutLivraison = Convert.ToDecimal(type.CoutLivraison);
                    objcommande.TypeLivraison = Convert.ToInt16(type.TypeLivraison);
                    objcommande.MontantTotAvantTaxes = Convert.ToDecimal(type.MontantTotAvantTaxes);
                    if (type.TPS != "") objcommande.TPS = Convert.ToDecimal(type.TPS);
                    if (type.TVQ != "") objcommande.TVQ = Convert.ToDecimal(type.TVQ);
                    objcommande.PoidsTotal = Convert.ToDecimal(type.PoidsTotal);
                    objcommande.Statut = Convert.ToChar(type.Statut);
                    objcommande.NoAutorisation = type.NoAutorisation;

                context.PPCommandes.InsertOnSubmit(objcommande);
                }
          
        
            /*
             * PPDetailsCommande
             */
            var docDetailsCommande = (from unDetailCommande in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPDetailsCommande.xml").Descendants("row")
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
                objDetailsCommande.PrixVente = Convert.ToDecimal(type.PrixVente);
                objDetailsCommande.Quantité = Convert.ToInt16(type.Quantité);

                context.PPDetailsCommandes.InsertOnSubmit(objDetailsCommande);
            }

            /*
             * PPHistoriquePayement
             */
            var docHistorique = (from uneHistorique in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPHistoriquePayement.xml").Descendants("row")
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
                objHistoriquePaiement.MontantVenteAvantLivraison = Convert.ToDecimal(unHistorique.MontantVenteAvantLivraison);
                objHistoriquePaiement.NoVendeur = Convert.ToInt64(unHistorique.NoVendeur);
                objHistoriquePaiement.NoClient = Convert.ToInt64(unHistorique.NoClient);
                objHistoriquePaiement.NoCommande = Convert.ToInt64(unHistorique.NoCommande);
                objHistoriquePaiement.DateVente = Convert.ToDateTime(unHistorique.DateVente);
                objHistoriquePaiement.NoAutorisation = unHistorique.NoAutoristion;
                objHistoriquePaiement.FraisLesi = Convert.ToDecimal(unHistorique.FraisLesi);
                objHistoriquePaiement.Redevance = Convert.ToDecimal(unHistorique.Redevance);
                if (unHistorique.FraisLivraison != "") objHistoriquePaiement.FraisLivraison = Convert.ToDecimal(unHistorique.FraisLivraison);
                if (unHistorique.FraisTPS != "") objHistoriquePaiement.FraisTPS = Convert.ToDecimal(unHistorique.FraisTPS);
                if (unHistorique.FraisTVQ != "") objHistoriquePaiement.FraisTVQ = Convert.ToDecimal(unHistorique.FraisTVQ);

              

                context.PPHistoriquePaiements.InsertOnSubmit(objHistoriquePaiement);

            }
            /*
             * PPLieu
             */
            var docLieu = (from unLieu in XDocument.Load(@"E:\\Gerald-Godin\\ProjetFinalPetitePUCE'S\\XML\\PPLieu.xml").Descendants("row")
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
            context.SubmitChanges();

            return Content(test);
        }


    }

}