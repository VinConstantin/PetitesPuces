using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Panier
    {
        [DisplayName("Nom du client")]
        public string NomClient { get; set; }

        [DisplayName("Date de création")]
        public DateTime DateCreation { get; set; }

        [DisplayName("Nombre d'items")]
        public int NbItems { get; set; }

        [DisplayName("Coût total")]
        public double CoutTotal { get; set; }
    }
}