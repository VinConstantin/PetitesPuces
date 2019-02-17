using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using PetitesPuces.Utilities;

namespace PetitesPuces.Models
{
    public class Panier
    {
        [DisplayName("Date de création")]
        public DateTime DateCreation { get; set; }
        
        public PPClient Client { get; set; }
        public PPVendeur Vendeur { get; set; }
        public List<PPArticlesEnPanier> Articles { get; set; }
        public bool EstEnRabais
        {
            get { return Articles.Any(a => a.PPProduit.DateVente >= DateTime.Now); }
        }

        
        public DateTime DateRabais
        {
            get
            {
                if (!Articles.Any())
                    throw new ArgumentException();
                return Articles.Where(a => a.PPProduit.EstEnRabais).Min(a => a.PPProduit.DateVente).Value;
            }
        }

        public decimal GetTPS(decimal prixLivraison)
        {
            if (!Vendeur.Taxes.GetValueOrDefault())
                return 0;
            
            return (getPrixTotal()+prixLivraison)*(decimal) 0.05;    
        }
        public decimal GetTVQ(decimal prixLivraison)
        {
            
            if (Vendeur.Province != "QC" || !Vendeur.Taxes.GetValueOrDefault())
                return 0;
            return (getPrixTotal() + prixLivraison) * (decimal) 0.0975;

        }
        public bool DepassePoidsMaximum
        {
            get { return GetPoidsTotal() > Vendeur.PoidsMaxLivraison; }
        }

        public decimal getPrixTotal()
        {
            return (decimal) Articles.Sum(a => (a.NbItems * a.PPProduit.GetPrixCourant()));
        }

        public int GetNbItems()
        {
            return (int)Articles.Sum(a => a.NbItems);
        }
        public int GetPoidsTotal()
        {
            return (int)Articles.Sum(a => a.NbItems * a.PPProduit.Poids);
        }
        public decimal? GetPrixSauveRabais()
        {
            decimal? montant = Articles.Sum(a =>
                (a.PPProduit.DateVente >= DateTime.Now)
                    ? a.NbItems * (a.PPProduit.PrixDemande - a.PPProduit.PrixVente)
                    : 0);

            return montant;
        }
    }
}