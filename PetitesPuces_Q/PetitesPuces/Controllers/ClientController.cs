using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Vendeur;

namespace PetitesPuces.Controllers
{
    public class ClientController : Controller
    {
        //TODO:implémenter pour utiliser le bon no
        private const int NOCLIENT = 10100;
        private const int DEFAULTITEMPARPAGE = 8;
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);
            List<PPCommande> lstCommandes = GetCommandesClient(NOCLIENT);
            
            AccueilViewModel viewModel = new AccueilViewModel
            {
                Paniers = lstPaniers,
                Commandes = lstCommandes
            };
            return View(viewModel);
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
            if (Vendeur == "-1" || String.IsNullOrEmpty(Vendeur) || !int.TryParse(Vendeur, out NoVendeur))
            {
                vendeur = new PPVendeur
                {
                    NoVendeur = -1
                };
                
                //creer la liste de produits
                listeProduits = (from p in context.PPProduits select p)
                    .Where(p => categorie == null || p.PPCategory == categorie);
            }
            else
            {
                
                var requete = (from unVendeur in context.PPVendeurs 
                    where unVendeur.NoVendeur == NoVendeur
                    select unVendeur);
                vendeur = requete.FirstOrDefault();
                //creer la liste de produits
                listeProduits = vendeur.PPProduits
                    .Where(p => categorie == null || p.PPCategory == categorie);
            }

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
            
            ViewBag.NoCategorie = viewModel.Categorie==null?-1 : viewModel.Categorie.NoCategorie;
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
            
            ViewBag.NoCategorie = viewModel.Categorie==null?-1 : viewModel.Categorie.NoCategorie;
            ViewBag.NbItems = nbItemsParPage;
            ViewBag.noPage = noPage;
            ViewBag.NbPage = (listeProduits.Count()-1)/nbItemsParPage+1;
            ViewBag.Filtre = Filtre;
            return PartialView("Client/_Catalogue",viewModel);
        }

        public ActionResult InformationProduit(int NoProduit)
        {
            PPProduit produit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit).FirstOrDefault();

            return PartialView("Client/ModalProduit", produit);

        }
        [HttpPost]
        public HttpStatusCode AjouterProduitAuPanier(int NoProduit, short Quantite)
        {
            var requeteProduit = (from unProduit in context.PPProduits
                where unProduit.NoProduit == NoProduit
                select unProduit);

            if (!requeteProduit.Any())
                return HttpStatusCode.Gone;

            int noVendeur = (int) requeteProduit.First().NoVendeur;
            DateTime dateCreation = DateTime.Now;
            short nbItems = Quantite;
            
            long noPanier = (from unPanier in context.PPArticlesEnPaniers
                select unPanier.NoPanier).Max() + 1;

            PPArticlesEnPanier article = new PPArticlesEnPanier
            {
                NoPanier = noPanier,
                NoClient = NOCLIENT,
                NoVendeur = noVendeur,
                DateCreation = dateCreation,
                NbItems = nbItems,
                NoProduit = NoProduit
            };        
            context.PPArticlesEnPaniers.InsertOnSubmit(article);            
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                return HttpStatusCode.InternalServerError;
            }         
            return HttpStatusCode.OK;
        }
        public ActionResult MonPanier(string No)
        {
            ViewBag.NoVendeur = No;
            List<Panier> lstPaniers = GetPaniersClient(NOCLIENT);
            //List<Panier> lstPaniers = new List<Panier>();
            return View(lstPaniers);
        }
        private List<Panier> GetPaniersClient(int NoClient)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NoClient
                orderby articles.DateCreation ascending
                group articles by articles.NoVendeur into g
                select g;

            var paniers = query.ToList();

            List<Panier> lstPaniers = new List<Panier>();

            foreach (var pan in paniers)
            {
                Panier panier = new Panier
                {
                    Client = pan.FirstOrDefault().PPClient,
                    Vendeur = pan.FirstOrDefault().PPVendeur,
                    DateCreation = (DateTime)pan.FirstOrDefault().DateCreation,
                    Articles = pan.ToList()
                };
                lstPaniers.Add(panier);
            }

            return lstPaniers;
        }
        private List<PPCommande> GetCommandesClient(int NoClient)
        {
            var query = from commande in context.PPCommandes
                where commande.NoClient == NoClient
                orderby commande.DateCommande ascending
                select commande;

            List<PPCommande> lstCommandes = query.ToList();


            return lstCommandes;
        }
        public ActionResult Commande(string Etape)
        {
            ViewBag.Etape = Etape;
            
            return View();
        }

        public ActionResult Information(int NoClient=0)
        {
            PPClient client = (from cli in context.PPClients
                where cli.NoClient == NOCLIENT
                select cli).FirstOrDefault();
            return PartialView("Client/Commande/_Information", client);
        }
        public ActionResult Livraison()
        {
            return PartialView("Client/Commande/_Livraison");
        }
        public ActionResult Paiement()
        {
            return PartialView("Client/Commande/_Paiement");
        }
        public ActionResult Confirmation()
        {
            Panier panier = new Panier
            {
                Vendeur = (from p in context.PPVendeurs select p).FirstOrDefault(),
                Client = (from p in context.PPClients select p).FirstOrDefault(),
                DateCreation = DateTime.Now,
                Articles = (from p in context.PPArticlesEnPaniers select p).Take(4).ToList()
            };
            return PartialView("Client/Commande/_Confirmation",panier);
        }

        public ActionResult DetailPanier(int noVendeur)
        {
            var query = from articles in context.PPArticlesEnPaniers
                where articles.NoClient == NOCLIENT
                      && articles.NoVendeur == noVendeur
                orderby articles.DateCreation ascending
                select articles;

            Panier panier = new Panier
            {
                Vendeur = query.FirstOrDefault().PPVendeur,
                Client = query.FirstOrDefault().PPClient,
                Articles = query.ToList()
            };
            return PartialView("Client/_DetailPanier",panier);
        }

        public ActionResult Profil()
        {
            return View();
        }
        public ActionResult modificationMDP(FormCollection formCollection)
        {
          /*  PPClient unClient=new PPClient();

            var motDePasseCourrant=formCollection[""]
            var nouveauMotdePasse;
            */
            return View();
        }

    }
}
