using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Commande
    {
        public int NoCommande { get; set; }
        public string NomClient { get; set; }
        public DateTime DateCommande { get; set; }
        public string Type { get; set; }
        public double TotalAvantTaxes { get; set; }
    }
}