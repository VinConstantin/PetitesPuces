using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Vendeur;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            ViewBag.Utilisateur = "Client";
            return View();
        }
        

        public ActionResult Catalogue(string Vendeur)
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
            
            var viewModel = new CatalogueViewModel
            {
                Vendeur = vendeur,
                Vendeurs = (from unVendeur in context.PPVendeurs
                    select unVendeur).ToList(),
                Categories = (from uneCategorie in context.PPCategories
                    select uneCategorie).ToList()              
            };

            return View(viewModel);
        }
        public ActionResult ListeProduits(string Vendeur, string Categorie)
        {
            PPCategory categorie;
            int NoCategorie;
            if (String.IsNullOrEmpty(Categorie) || !int.TryParse(Categorie, out NoCategorie))
            {
                categorie = null;
            }
            else
            {
                var requeteCategorie = (from uneCategorie in context.PPCategories 
                    where uneCategorie.NoCategorie == NoCategorie
                    select uneCategorie);
                categorie = requeteCategorie.FirstOrDefault();
            }
            
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
            
            var viewModel = new CatalogueViewModel
            {
                Vendeur = vendeur,
                Vendeurs = (from unVendeur in context.PPVendeurs
                    select unVendeur).ToList(),
                Categories = (from uneCategorie in context.PPCategories
                    select uneCategorie).ToList()              ,
                Categorie = categorie
            };
            

            return View("Client/_Catalogue",viewModel);
        }
        public ActionResult MonPanier(string No)
        {
            ViewBag.Message = "Votre panier No. "+No;
            
            return View();
        }
        public ActionResult Commande(string Etape)
        {
            ViewBag.Etape = Etape;
            
            return View();
        }

        public ActionResult Profil()
        {
            return View();
        }
        public ActionResult modificationMDP()
        {
            return View();
        }

    }
}
