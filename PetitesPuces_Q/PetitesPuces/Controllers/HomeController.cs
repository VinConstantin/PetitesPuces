
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Home;

namespace PetitesPuces.Controllers
{
    public class HomeController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            AccueilHomeViewModel viewModel = new AccueilHomeViewModel
            {
                Produits = (from p in context.PPProduits
                    orderby p.DateCreation
                    select p).Take(12).ToList(),
                Vendeurs = (from v in context.PPVendeurs select v).ToList(),
                Categories = (from c in context.PPCategories select c).ToList()
                
            };

            return View(viewModel);
        }

        public ActionResult ListeProduits()
        {
            List<PPProduit> produits = (from p in context.PPProduits
                orderby p.DateCreation
                select p).Take(12).ToList();

            return View("Home/_ListeProduits", produits);
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            return View();
        }
        public ActionResult Deconnexion()
        {
            HttpContext.Session["userId"] = null;           
            
            return RedirectToAction("Index","Home");
        }


        [HttpPost]
        public ActionResult Connexion(FormCollection formCollection)
        {

            var unClientExist = from unClient in context.PPClients
                                where unClient.AdresseEmail == formCollection["adresseEmail"] &&
                                      unClient.MotDePasse == formCollection["motDePasse"]
                                select unClient;

            var unVendeurExist = from unVendeur in context.PPVendeurs
                                 where unVendeur.AdresseEmail == formCollection["adresseEmail"] &&
                                       unVendeur.MotDePasse == formCollection["motDePasse"]
                                 select unVendeur;

            var unGestionnaireExist = from unGestionnaire in context.PPGestionnaires
                                      where unGestionnaire.AdresseEmail == formCollection["AdresseEmail"]
                                      select unGestionnaire;


            if (unClientExist.Count() != 0)
            {
                System.Web.HttpContext.Current.Session["userId"] = unClientExist.First().NoClient;
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Client");
            }
            else if (unVendeurExist.Count() != 0)
            {
                System.Web.HttpContext.Current.Session["userId"] = unVendeurExist.First().NoVendeur;
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Vendeur");
            }
            else if (unGestionnaireExist.Count() != 0)
            {
                System.Web.HttpContext.Current.Session["userId"] = unGestionnaireExist.First().NoGestionnaire;
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Gestionnaire");
            }
            else
            {
                ModelState.AddModelError("motDePasse", "Votre mot de passe ou adresse courriel est incorrect.");
            }


            return View();
        }


        [HttpGet]
        public ActionResult InscriptionClient()
        {


            return View();
        }


        [HttpPost]
        public ActionResult InscriptionClient(FormCollection formCollection)
        {
            /*foreach (var key in formCollection.AllKeys)
            {
                Response.Write("key: " + key + " ");
                Response.Write(formCollection[key]);
                Response.Write("<br/> ");
            }*/

            if (ModelState.IsValid)
            {

                PPClient nouveauClient = new PPClient();
                var noClientCourrant = from unclient in context.PPClients select unclient.NoClient;
                int maxNo = Convert.ToInt16(noClientCourrant.Max()) + 1;

                var VerificationAdresseCourriel = from unclient in context.PPClients
                                                  where unclient.AdresseEmail == formCollection["AdresseEmail"]
                                                  select unclient;

                if (VerificationAdresseCourriel.Count() == 0)
                {



                    nouveauClient.AdresseEmail = formCollection["AdresseEmail"];

                    nouveauClient.MotDePasse = formCollection["MotDePasse"];
                    nouveauClient.NoClient = maxNo;


                    context.PPClients.InsertOnSubmit(nouveauClient);
                    context.SubmitChanges();
                    return RedirectToAction("Connexion", "Home");
                }
                else
                {
                    ModelState.AddModelError("AdresseEmail", "Cette adresse courriel est déjà utilisée, veuillez réessayer un nouveau!");

                }
            }

            return View();
        }
        [HttpGet]
        public ActionResult InscriptionVendeur()
        {

            return View();
        }

        [HttpPost]
        public ActionResult InscriptionVendeur(FormCollection formCollection)
        {

            /* foreach (var key in formCollection.AllKeys)
             {
                 Response.Write("key: " + key + " ");
                 Response.Write(formCollection[key]);
                 Response.Write("<br/> ");
             }*/

            if (ModelState.IsValid)
            {

                var noVendeurCourrant = from unVendeur in context.PPVendeurs select unVendeur.NoVendeur;
                int maxNoVendeur = Convert.ToInt16(noVendeurCourrant.Max()) + 1;
                var VerificationAdresseCourriel = from unVendeur in context.PPVendeurs
                                                  where unVendeur.AdresseEmail == formCollection["AdresseEmail"]
                                                  select unVendeur;

                PPVendeur nouveauVendeur = new PPVendeur();

                if (VerificationAdresseCourriel.Count() == 0)
                {

                    DateTime today = DateTime.Today;
                    nouveauVendeur.NomAffaires = formCollection["NomAffaires"];
                    nouveauVendeur.Nom = formCollection["Nom"];
                    nouveauVendeur.Prenom = formCollection["Prenom"];
                    nouveauVendeur.Rue = formCollection["Rue"];
                    nouveauVendeur.NoVendeur = maxNoVendeur;
                    nouveauVendeur.Ville = formCollection["Ville"];
                    nouveauVendeur.Province = formCollection["Province"];
                    nouveauVendeur.CodePostal = formCollection["CodePostal"];
                    nouveauVendeur.Tel1 = formCollection["tel1"];
                    nouveauVendeur.Tel2 = formCollection["tel2"];
                    nouveauVendeur.AdresseEmail = formCollection["AdresseEmail"];
                    nouveauVendeur.PoidsMaxLivraison = Convert.ToInt32(formCollection["poidsMax"]);
                    nouveauVendeur.LivraisonGratuite = Convert.ToDecimal(formCollection["prixMinimum"]);
                    nouveauVendeur.MotDePasse = formCollection["motDePasse"];
                    nouveauVendeur.Taxes = formCollection["taxes"] == null ? false : true;
                    nouveauVendeur.DateCreation = today;
                    nouveauVendeur.Pays = formCollection["Pays"];


                    context.PPVendeurs.InsertOnSubmit(nouveauVendeur);
                    context.SubmitChanges();
                    return RedirectToAction("Connexion", "Home");
                }
                else
                {
                    ModelState.AddModelError("AdresseEmail", "Cette adresse courriel est déjà utilisée, veuillez réessayer un nouveau!");
                }
            }
            return View();
        }

        public ActionResult OubliMDP()
        {
            return View();
        }
        public ActionResult Catalogue()
        {
            return RedirectToAction("Catalogue", "Client");


        }

        public ActionResult testValidation()

        {
            return View();
        }
    }
}