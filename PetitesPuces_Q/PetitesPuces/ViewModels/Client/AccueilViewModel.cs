using System.Collections.Generic;

namespace PetitesPuces.Models
{
    public class AccueilViewModel
    {
        public List<Panier> Paniers { get; set; }
        public List<PPCommande> Commandes { get; set; }
    }
}