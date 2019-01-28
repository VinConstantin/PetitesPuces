using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Home
{
    public class AccueilHomeViewModel
    {
        private BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        public PPVendeur Vendeur { get; set; }
        public List<PPCategory> Categories { get; set; }
        public List<PPProduit> Produits { get; set; }
        public List<PPVendeur> Vendeurs { get; set; }


        public AccueilHomeViewModel(PPVendeur vendeur)
        {
            Vendeur = vendeur;
            Categories = new List<PPCategory>();
            Produits = new List<PPProduit>();
            Vendeurs = new List<PPVendeur>();

            var requeteCategories = from uneCategorie in context.PPCategories
                select uneCategorie;
            Categories = requeteCategories.ToList();

            var requeteVendeurs = from unVedeur in context.PPVendeurs
                select unVedeur;
            Vendeurs = requeteVendeurs.ToList();
            
            Produits = Vendeur.PPProduits.ToList();
        }
    }
}