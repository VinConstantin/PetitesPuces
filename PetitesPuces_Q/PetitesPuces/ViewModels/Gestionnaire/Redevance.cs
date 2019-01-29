using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class Redevance
    {
        public PPVendeur Vendeur { get; set; }
        public decimal Solde { get; set; }
        public DateTime EnSouffranceDepuis { get; set; }
    }
}      