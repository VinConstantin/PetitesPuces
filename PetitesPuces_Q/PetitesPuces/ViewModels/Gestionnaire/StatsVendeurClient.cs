using System;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class StatsVendeurClient
    {
        public SerializablePPVendeur Vendeur { get;set;}
        public decimal TotalCommandes { get; set; }
        public decimal TotalBrut { get; set; }
        public decimal TotalTaxesEtLivraison { get; set; }
        public DateTime DateDerniereCommande { get; set; }
    }
}