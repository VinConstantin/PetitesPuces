using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Vendeur
    {
        [DisplayName("No Vendeur")]
        public int NoVendeur { get; set; }

        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        [DisplayName("Nom")]
        public string Nom { get; set; }

        [DisplayName("Prénom")]
        public string Prenom { get; set; }

        [DisplayName("Date de la demande")]
        public DateTime DateCreation { get; set; }
    }
}