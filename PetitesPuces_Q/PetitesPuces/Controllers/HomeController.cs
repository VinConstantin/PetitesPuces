using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Home;

namespace PetitesPuces.Controllers
{
    public class HomeController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        public ActionResult Index(string Vendeur)
        {
            PPVendeur vendeur;
            int NoVendeur;
            if (String.IsNullOrEmpty(Vendeur) || !int.TryParse(Vendeur, out NoVendeur))
            {
                var requete = (from unVendeur in context.PPVendeurs select unVendeur);
                vendeur = requete.FirstOrDefault();
            }
            else
            {
                var requete = (from unVendeur in context.PPVendeurs
                               where unVendeur.NoVendeur == NoVendeur
                               select unVendeur);
                vendeur = requete.FirstOrDefault();
            }

            AccueilHomeViewModel viewModel = new AccueilHomeViewModel(vendeur);

            return View(viewModel);
        }
        public ActionResult ListeProduits(string Vendeur)
        {
            PPVendeur vendeur;
            int NoVendeur;
            if (string.IsNullOrEmpty(Vendeur) || !int.TryParse(Vendeur, out NoVendeur))
            {
                var requete = (from unVendeur in context.PPVendeurs select unVendeur);
                vendeur = requete.FirstOrDefault();
            }
            else
            {
                var requete = (from unVendeur in context.PPVendeurs
                               where unVendeur.NoVendeur == NoVendeur
                               select unVendeur);
                vendeur = requete.FirstOrDefault();
            }

            return View("Home/_ListeProduits", vendeur.PPProduits.ToList());
        }

        [HttpGet]
        public ActionResult Connexion()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Connexion(FormCollection formCollection)
        {
            var unClientExist = from unClient in context.PPClients
                                where unClient.AdresseEmail == formCollection["usernameLogin"] &&
                                      unClient.MotDePasse == formCollection["passwordLogin"]
                                select unClient;

            var unVendeurExist = from unVendeur in context.PPVendeurs
                                 where unVendeur.AdresseEmail == formCollection["usernameLogin"] &&
                                       unVendeur.MotDePasse == formCollection["passwordLogin"]
                                 select unVendeur;

            if (unClientExist.Count() != 0)
            {
                HttpContext.Session["userId"] = unClientExist.First().NoClient;
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Client");
            }
            else if (unVendeurExist.Count() != 0)
            {
                HttpContext.Session["userId"] = unVendeurExist.First().NoVendeur;
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Vendeur");
            }

            foreach (var key in formCollection.AllKeys)
            {
                Response.Write("key: " + key + " ");
                Response.Write(formCollection[key]);
                Response.Write("<br/> ");
            }

            var noClientCourrant = from unclient in context.PPClients select unclient.NoClient;
            int maxNo = Convert.ToInt16(noClientCourrant.Max()) + 1;

            PPClient nouveauClient = new PPClient();
            nouveauClient.Nom = formCollection["username"];
            nouveauClient.AdresseEmail = formCollection["emailClient"];
            nouveauClient.MotDePasse = formCollection["passwordClient"];
            nouveauClient.NoClient = maxNo;
            UpdateModel(nouveauClient);
            Response.Write(nouveauClient.Nom);
            context.PPClients.InsertOnSubmit(nouveauClient);

            var noVendeurCourrant = from unVendeur in context.PPVendeurs select unVendeur.NoVendeur;
            int maxNoVendeur = Convert.ToInt16(noVendeurCourrant.Max()) + 1;

            PPVendeur nouveauVendeur = new PPVendeur();

            DateTime today = DateTime.Today;
            nouveauVendeur.NomAffaires = formCollection["nomAffaires"];
            nouveauVendeur.Nom = formCollection["nomVendeur"];
            nouveauVendeur.Prenom = formCollection["prenomVendeur"];
            nouveauVendeur.Rue = formCollection["rue"];
            nouveauVendeur.NoVendeur = maxNoVendeur;
            nouveauVendeur.Ville = formCollection["ville"];
            nouveauVendeur.Province = formCollection["province"];
            nouveauVendeur.CodePostal = formCollection["codePostal"];
            nouveauVendeur.Tel1 = formCollection["tel1"];
            nouveauVendeur.Tel2 = formCollection["tel2"];
            nouveauVendeur.AdresseEmail = formCollection["adresseEmail"];
            nouveauVendeur.PoidsMaxLivraison = Convert.ToInt32(formCollection["poidMaxLiv"]);
            nouveauVendeur.LivraisonGratuite = Convert.ToDecimal(formCollection["livraisonGraduite"]);
            nouveauVendeur.MotDePasse = formCollection["MDP"];
            nouveauVendeur.Taxes = formCollection["taxe"]== null ? false : true;
            nouveauVendeur.DateCreation = today;
            nouveauVendeur.Pays = formCollection["pays"];
            // UpdateModel(nouveauVendeur);
            Response.Write(nouveauVendeur.Nom);
            context.PPVendeurs.InsertOnSubmit(nouveauVendeur);

            context.SubmitChanges();

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
    }
}