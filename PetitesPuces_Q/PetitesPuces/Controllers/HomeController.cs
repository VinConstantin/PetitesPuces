using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Home;

namespace TestMVC.Controllers
{
    public class HomeController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        public ActionResult Index(string Categorie, string Vendeur)
        {
            int NoCategorie;
            int NoVendeur;
            if (String.IsNullOrEmpty(Categorie) || !int.TryParse(Categorie, out NoCategorie))
                NoCategorie = 1;
            if (String.IsNullOrEmpty(Vendeur) || !int.TryParse(Vendeur, out NoVendeur))
                NoVendeur = 1;
            
            AccueilHomeViewModel viewModel = new AccueilHomeViewModel(
                new Categorie{No = NoCategorie , Description = "Sport", Details = "Tous les articles de sport"},
                new Vendeur{Nom = "Ginette",Prenom = "Francois",NoVendeur = NoVendeur,NomAffaires = "Clémentines",AdresseEmail = "courriel@gmail.com",DateCreation = new DateTime()}
                );
            
            return View(viewModel);
        }

        public ActionResult Connexion()
        {
            var query = from whatever in context.PPClients
                select whatever;
            var clients = query.ToList();
            return View(clients);
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