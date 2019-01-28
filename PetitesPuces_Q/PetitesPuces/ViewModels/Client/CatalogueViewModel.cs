using System.Collections.Generic;

namespace PetitesPuces.Models
{
    public class CatalogueViewModel
    {
        public List<PPCategory> Categories { get; set; }
        public List<PPVendeur> Vendeurs { get; set; }
        public List<PPProduit> Produits { get; set; }
        public PPVendeur Vendeur { get; set; }
        public PPCategory Categorie { get; set; }
    }
}