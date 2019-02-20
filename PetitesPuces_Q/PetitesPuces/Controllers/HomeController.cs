
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using PetitesPuces.Models;
using PetitesPuces.ViewModels;
using PetitesPuces.ViewModels.Home;
using System.IO;
using IronPdf;
using PetitesPuces.Securite;

namespace PetitesPuces.Controllers
{
    public class HomeController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        public static string courrielOublieMDP;
        
        public ActionResult Index()
        {
            AccueilHomeViewModel viewModel = new AccueilHomeViewModel
            {
                Produits = (from p in context.PPProduits
                    orderby p.DateCreation
                    where p.Disponibilité == true
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

        public ActionResult Deconnexion()
        {
            HttpContext.Session["userId"] = null;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Connexion(string Status = "")
        {
            if (Request.Cookies["courriel"] != null && Request.Cookies["mdp"] != null)
            {
                ViewBag.courriel = Request.Cookies["courriel"].Value;
                ViewBag.mdp = Request.Cookies["mdp"].Value;
                ViewBag.souvenirCheck = "checked";
            }
            else ViewBag.souvenirCheck = "";

            ViewBag.Status = Status;
            return View();
        }

        [HttpPost]
        public ActionResult Connexion(FormCollection formCollection)
        {
            var unClientExist = from unClient in context.PPClients
                where unClient.AdresseEmail == formCollection["adresseEmail"] &&
                      unClient.MotDePasse == formCollection["motDePasse"] &&
                      unClient.Statut == (int)StatutCompte.ACTIF
                select unClient;

            var unVendeurExist = from unVendeur in context.PPVendeurs
                where unVendeur.AdresseEmail == formCollection["adresseEmail"] &&
                      unVendeur.MotDePasse == formCollection["motDePasse"] &&
                      unVendeur.Statut == (int)StatutCompte.ACTIF
                select unVendeur;

            var unGestionnaireExist = from unGestionnaire in context.PPGestionnaires
                where unGestionnaire.AdresseEmail == formCollection["AdresseEmail"]
                select unGestionnaire;

            if (unClientExist.Count() != 0 || unVendeurExist.Count() != 0 || unGestionnaireExist.Count() != 0)
            {
                if (formCollection["remember"] == "on")
                {
                    Response.Cookies["courriel"].Value = formCollection["adresseEmail"];
                    Response.Cookies["mdp"].Value = formCollection["motDePasse"];
                    Response.Cookies["courriel"].Expires = DateTime.Now.AddYears(1);
                    Response.Cookies["mdp"].Expires = DateTime.Now.AddYears(1);
                }
                else
                {
                    Response.Cookies["courriel"].Expires = DateTime.Now.AddYears(-1);
                    Response.Cookies["mdp"].Expires = DateTime.Now.AddYears(-1);
                }

                if (unClientExist.Count() != 0)
                {
                    System.Web.HttpContext.Current.Session["userId"] = unClientExist.First().NoClient;
                    unClientExist.First().DateDerniereConnexion = DateTime.Now;
                    unClientExist.First().NbConnexions++;
                    TempData["connexion"] = true;
                    try
                    {
                        context.SubmitChanges();
                        return RedirectToAction("Index", "Client");
                    }
                    catch (Exception e)
                    {
                    }
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
            }
            else
            {
                ModelState.AddModelError("motDePasse", "Votre mot de passe ou adresse courriel est incorrect.");
            }

            return View();
        }


        [HttpGet]
        public ActionResult InscriptionClient(string nouveauUtilisateur="")
        {
            ViewBag.nouveauUtilisateur = nouveauUtilisateur;
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
                    nouveauClient.DateCreation = DateTime.Now;
                    nouveauClient.Statut = 1;
                    try
                    {
                        context.PPClients.InsertOnSubmit(nouveauClient);
                        context.SubmitChanges();
                        return RedirectToAction("Connexion", "Home", new {Status = "InscriptionReussi"});
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    ModelState.AddModelError("AdresseEmail",
                        "Cette adresse courriel est déjà utilisée, veuillez réessayer un nouveau!");
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
            /*
             foreach (var key in formCollection.AllKeys)
            {
                Response.Write("key: " + key + ": ");
                Response.Write(formCollection[key] + ",  type:/");
                Response.Write(formCollection[key].GetType() + "/");
                Response.Write("<br/> ");
            }
            */

            if (ModelState.IsValid)
            {
                var tousLesVendeurs = from unVendeur in context.PPVendeurs select unVendeur.NoVendeur;

                int maxNoVendeur = Convert.ToInt32(tousLesVendeurs.Max()) + 1;

                var VerificationAdresseCourriel = from unVendeur in context.PPVendeurs
                    where unVendeur.AdresseEmail == formCollection["AdresseEmail"]
                    select unVendeur;

                PPVendeur nouveauVendeur = new PPVendeur();

                if (VerificationAdresseCourriel.Count() == 0)
                {
                    nouveauVendeur.NoVendeur = maxNoVendeur;
                    nouveauVendeur.NomAffaires = formCollection["Vendeur.NomAffaires"];
                    nouveauVendeur.Nom = formCollection["Vendeur.Nom"];
                    nouveauVendeur.Prenom = formCollection["Vendeur.Prenom"];
                    nouveauVendeur.Rue = formCollection["Vendeur.Rue"];
                    nouveauVendeur.Ville = formCollection["Vendeur.Ville"];
                    nouveauVendeur.Province = formCollection["Vendeur.Province"];
                    nouveauVendeur.CodePostal = formCollection["Vendeur.CodePostal"];
                    nouveauVendeur.Pays = formCollection["Vendeur.Pays"];
                    nouveauVendeur.Tel1 = formCollection["Vendeur.Tel1"];
                    nouveauVendeur.Tel2 = formCollection["Vendeur.Tel2"];
                    nouveauVendeur.AdresseEmail = formCollection["AdresseEmail"];
                    nouveauVendeur.PoidsMaxLivraison = Convert.ToInt32(formCollection["Vendeur.PoidsMaxLivraison"]);
                    nouveauVendeur.LivraisonGratuite = Convert.ToDecimal(formCollection["Vendeur.LivraisonGratuite"]);
                    nouveauVendeur.MotDePasse = formCollection["MotDePasse"];
                    nouveauVendeur.Taxes = formCollection["Taxes"] == "on" ? true : false;
                    nouveauVendeur.DateCreation = DateTime.Now;

                    nouveauVendeur.Statut = 0;
                    try
                    {
                        context.PPVendeurs.InsertOnSubmit(nouveauVendeur);
                        context.SubmitChanges();
                        return RedirectToAction("Connexion", "Home", new {Status = "InscriptionReussi"});
                    }
                    catch (Exception e)
                    {
                    }
                }
                else
                {
                    ModelState.AddModelError("AdresseEmail",
                        "Cette adresse courriel est déjà utilisée, veuillez réessayer un nouveau!");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult OubliMDP(FormCollection formCollection)
        {
            VerificationCourriel verificationCourriel = new VerificationCourriel();
            verificationCourriel.AdresseEmail = formCollection["AdresseEmail"];
            
            return View(verificationCourriel);
        }

        [HttpPost]
        public ActionResult OubliMDP(VerificationCourriel verificationCourriel)
        {
            if (ModelState.IsValid)
            {
                 courrielOublieMDP = verificationCourriel.AdresseEmail;
           
                var objClient = (from var in context.PPClients
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                var objVendeur = (from var in context.PPVendeurs
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                var objGestionnaire = (from var in context.PPGestionnaires
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                if (objClient.Count() != 0 || objVendeur.Count() != 0 || objGestionnaire.Count() != 0)
                {
                    return RedirectToAction("courrielMDPOublie",new {courriel=courrielOublieMDP});
                }
                else
                {
                    return RedirectToAction("InscriptionClient",new{nouveauUtilisateur= "Cette adresse courriel n'est pas été enregistrée, vous pouvez créer un nouveau compte ici."});
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult modiOubliMDP(FormCollection formCollection )
        {
            ViewBag.check = "hi "+courrielOublieMDP;
            OubliMotDePasse oubliMotDePasse = new OubliMotDePasse
            {
                motDePass = formCollection["motDePass"]
           
            };
            return View(oubliMotDePasse);
        }

        [HttpPost]
        public ActionResult modiOubliMDP(OubliMotDePasse oubliMotDePasse)
        {
           
        
            if (ModelState.IsValid)
            {
                var objClient = (from var in context.PPClients
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                var objVendeur = (from var in context.PPVendeurs
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                var objGestionnaire = (from var in context.PPGestionnaires
                    where var.AdresseEmail == courrielOublieMDP
                    select var);
                try
                {
                    if (objClient.Count() != 0)
                    {
                        objClient.First().MotDePasse = oubliMotDePasse.motDePass;
                        context.SubmitChanges();
                    }
                    else if (objVendeur.Count() != 0)
                    {
                        objVendeur.First().MotDePasse = oubliMotDePasse.motDePass;
                        context.SubmitChanges();
                    }
                    else if (objGestionnaire.Count() != 0)
                    {
                        objGestionnaire.First().MotDePasse = oubliMotDePasse.motDePass;
                        context.SubmitChanges();
                    }

                }
                catch (Exception e)
                {
                }
                return RedirectToAction("Connexion",new{status="EnregistrementReussi"});
            }

            return View();
        }

        public ActionResult courrielMDPOublie(string courriel="")
        {
            ViewBag.courrielOublie = courriel;
            
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