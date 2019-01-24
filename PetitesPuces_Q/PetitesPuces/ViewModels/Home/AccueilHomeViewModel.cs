using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Home
{
    public class AccueilHomeViewModel
    {
        public Categorie Categorie { get; set; }
        public Vendeur Vendeur { get; set; }
        public List<Product> Produits { get; set; }
        public List<Categorie> Categories { get; set; }
        public List<Vendeur> Vendeurs { get; set; }


        public AccueilHomeViewModel(Categorie categorie,Vendeur vendeur)
        {
            Categorie = categorie;
            Vendeur = vendeur;
            Categories = new List<Categorie>();
            Produits = new List<Product>();
            Vendeurs = new List<Vendeur>();
            for(int i = 1; i <= 10; i ++)
            {
                Categories.Add(new Categorie{No = i, Description = "Sport", Details = "Tous les articles de sports"});
            };
            for(int i = 1; i <= 12; i ++)
            {
                Produits.Add(new Product(i, "Produit No."+i){Price = i*123});
            };
            for(int i = 1; i <= 10; i ++)
            {
                Vendeurs.Add(new Vendeur {Nom = "Bob",Prenom = "Graton",NoVendeur = i,NomAffaires = "Petites machine",AdresseEmail = "asd@asd.com",DateCreation = new DateTime()});
            };
        }
    }
}