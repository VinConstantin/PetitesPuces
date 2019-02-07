using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class GestionCatalogueViewModel
    {
        public List<PPCategory> Categories { get; set; }
        public List<PPProduit> Produits { get; set; }
    }
}