using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.VMVendeur;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        private const int DEFAULTITEMPARPAGE = 8;
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            ViewBag.Utilisateur = "Client";
            return View();
        }
        

        public ActionResult Catalogue(string Vendeur, string Categorie, string Page, string Size)
        {
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;

            //chercher la categorie
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
            //chercher le vendeur
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
            //creer le view model
            var viewModel = new CatalogueViewModel
            {
                Vendeur = vendeur,
                Vendeurs = (from unVendeur in context.PPVendeurs
                    select unVendeur).ToList(),
                Categories = (from uneCategorie in context.PPCategories
                    select uneCategorie).ToList(),
                Categorie = categorie,
                Produits = vendeur.PPProduits.Where(p => categorie==null || p.PPCategory == categorie).Skip(noPage*nbItemsParPage-nbItemsParPage).Take(nbItemsParPage).ToList()
            };
            ViewBag.NoCategorie = categorie?.NoCategorie ?? -1;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (vendeur.PPProduits.Where(p => categorie==null || p.PPCategory == categorie).ToList().Count)/nbItemsParPage+1;
            return View(viewModel);
        }
        public ActionResult ListeProduits(string Vendeur, string Categorie, string Page, string Size)
        {
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            
            //chercher la categorie
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
            //chercher le vendeur
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
            //creer le viewmodel
            var viewModel = new CatalogueViewModel
            {
                Vendeur = vendeur,
                Vendeurs = (from unVendeur in context.PPVendeurs
                    select unVendeur).ToList(),
                Categories = (from uneCategorie in context.PPCategories
                    select uneCategorie).ToList(),
                Categorie = categorie,
                Produits = vendeur.PPProduits.Where(p => categorie==null || p.PPCategory == categorie).Skip(noPage*nbItemsParPage-nbItemsParPage).Take(nbItemsParPage).ToList()
            };
            ViewBag.NoCategorie = categorie?.NoCategorie ?? -1;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (vendeur.PPProduits.Where(p => categorie==null || p.PPCategory == categorie).ToList().Count)/nbItemsParPage+1;
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
