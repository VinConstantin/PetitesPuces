using System;
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

            return View("Home/_ListeProduits",vendeur.PPProduits.ToList());
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