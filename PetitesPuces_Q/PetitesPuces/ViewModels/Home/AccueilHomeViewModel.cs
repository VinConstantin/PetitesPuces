using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Home
{
    public class AccueilHomeViewModel
    {
        public List<PPCategory> Categories { get; set; }
        public List<PPProduit> Produits { get; set; }
        public List<PPVendeur> Vendeurs { get; set; }
    }
}