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
                                where unClient.AdresseEmail == formCollection["adresseEmail"] &&
                                      unClient.MotDePasse == formCollection["motDePasse"]
                                select unClient;

            var unVendeurExist = from unVendeur in context.PPVendeurs
                                 where unVendeur.AdresseEmail == formCollection["adresseEmail"] &&
                                       unVendeur.MotDePasse == formCollection["motDePasse"]
                                 select unVendeur;




            if (unClientExist.Count() != 0)
            {
                TempData["connexion"] = true;
                return RedirectToAction("Index", "Client");
            }
            else if (unVendeurExist.Count() != 0)
            {

                TempData["connexion"] = true;
                return RedirectToAction("Index", "Vendeur");
            }
            else
            {
                ModelState.AddModelError("motDePasse","Votre mot de passe ou adresse courriel est incorrect.");
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

            if (ModelState.IsValid)
            {


                var noClientCourrant = from unclient in context.PPClients select unclient.NoClient;
                int maxNo = Convert.ToInt16(noClientCourrant.Max()) + 1;



                PPClient nouveauClient = new PPClient();
                nouveauClient.Nom = formCollection["username"];
                nouveauClient.AdresseEmail = formCollection["emailClient"];
                nouveauClient.MotDePasse = formCollection["passwordClient"];
                nouveauClient.NoClient = maxNo;
                UpdateModel(nouveauClient);
            
                context.PPClients.InsertOnSubmit(nouveauClient);
                context.SubmitChanges();
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
                
                PPVendeur nouveauVendeur = new PPVendeur();

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