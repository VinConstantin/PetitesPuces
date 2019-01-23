using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Commande
    {
        [DisplayName("No Commande")]
        public int NoCommande { get; set; }

        [DisplayName("Nom du client")]
        public string NomClient { get; set; }

        [DisplayName("Date de commande")]
        public DateTime DateCommande { get; set; }

        [DisplayName("Coût de livraison")]
        public double CoutLivraison { get; set; }

        [DisplayName("Type de livraison")]
        public string Type { get; set; }

        [DisplayName("Poids total")]
        public double PoidsTotal { get; set; }

        [DisplayName("Statut de livraison")]
        public char Statut { get; set; }

        [DisplayName("Total avant taxes")]
        public double TotalAvantTaxes { get; set; }
    }
}