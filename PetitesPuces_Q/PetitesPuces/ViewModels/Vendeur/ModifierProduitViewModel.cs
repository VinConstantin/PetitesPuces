using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class ModifierProduitViewModel
    {
        public PPProduit Produit { get; set; }
        public List<PPCategory> Categories { get; set; }
    }
}