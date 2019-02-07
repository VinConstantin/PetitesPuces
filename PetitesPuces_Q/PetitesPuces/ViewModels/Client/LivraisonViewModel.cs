using System.Collections.Generic;

namespace PetitesPuces.Models
{
    public class LivraisonViewModel
    {
        public Panier Panier { get; set; }
        public List<PPPoidsLivraison> livraison { get; set; }
        public decimal prixLivraison { get; set; }
    }
}