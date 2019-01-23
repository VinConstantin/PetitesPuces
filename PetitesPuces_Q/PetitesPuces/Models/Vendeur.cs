using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Vendeur: IPersonne
    {
        [DisplayName("No Vendeur")]
        public int NoVendeur { get; set; }

        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        public string AdresseEmail { get; set; }

        [DisplayName("Nom")]
        public string Nom { get; set; }

        [DisplayName("Prénom")]
        public string Prenom { get; set; }

        [DisplayName("Date de la demande")]
        public DateTime DateCreation { get; set; }

        public DateTime DateMAJ { get; }

        public string Rue { get; }
        public string Ville { get; }
        public string Province { get; }
        public string CodePostal { get; }
        public string Pays { get; }

        public string Role
        {
            get { return "Vendeur"; }
        }
    }
}