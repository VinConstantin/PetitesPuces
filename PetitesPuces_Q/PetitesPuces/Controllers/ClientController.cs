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

        private CatalogueViewModel GetCatalogueViewModel(ref IEnumerable<PPProduit> listeProduits, 
            string Vendeur, string Categorie, string Page, string Size,
            string Filtre, Tri tri)
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
            //creer la liste de produits
            listeProduits = vendeur.PPProduits
                .Where(p => categorie == null || p.PPCategory == categorie);
            if(!String.IsNullOrEmpty(Filtre)) 
                listeProduits = listeProduits.Where(p => p.Nom.ToLower().Contains(Filtre.ToLower()));

            switch (tri)
            {
                case Tri.Nom :
                    listeProduits = listeProduits.OrderBy(p => p.Nom);
                    break;
                case Tri.Categorie :
                    listeProduits = listeProduits.OrderBy(p => p.PPCategory.Description);
                    break;
                case Tri.Date :
                    listeProduits = listeProduits.OrderBy(p => p.DateCreation);
                    break;
                case Tri.Numero :
                    listeProduits = listeProduits.OrderBy(p => p.NoProduit);
                break;
                default:
                    listeProduits = listeProduits.OrderBy(p => p.Nom);
                    break;
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
                Produits = listeProduits.Skip(noPage * (nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage) -
                                              (nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage))
                    .Take((nbItemsParPage == -1 ? listeProduits.Count() : nbItemsParPage)).ToList()
            };
            return viewModel;
        }
        public ActionResult Catalogue(string Vendeur, string Categorie, string Page, string Size, string Filtre, string TriPar)
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);
            
            CatalogueViewModel viewModel = GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);
            
            ViewBag.NoCategorie = viewModel.Categorie?.NoCategorie ?? -1;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count()-1)/nbItemsParPage+1;
            ViewBag.Filtre = Filtre;
            return View(viewModel);
        }
        public ActionResult ListeProduits(string Vendeur, string Categorie, string Page, string Size, string Filtre, string TriPar)
        {
            IEnumerable<PPProduit> listeProduits = new List<PPProduit>();
            int noPage = int.TryParse(Page, out noPage) ? noPage : 1;
            int nbItemsParPage = int.TryParse(Size, out nbItemsParPage) ? nbItemsParPage : DEFAULTITEMPARPAGE;
            Tri tri;
            Enum.TryParse(TriPar, out tri);
            
            CatalogueViewModel viewModel = GetCatalogueViewModel(ref listeProduits, Vendeur, Categorie, Page, Size, Filtre, tri);
            
            ViewBag.NoCategorie = viewModel.Categorie?.NoCategorie ?? -1;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count()-1)/nbItemsParPage+1;
            ViewBag.Filtre = Filtre;
            return PartialView("Client/_Catalogue",viewModel);
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
