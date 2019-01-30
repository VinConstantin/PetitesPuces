using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class AccueilVendeurViewModel
    {
        public List<PPCommande> Commandes { get; set; }
        public List<Panier> Paniers { get; set; }
        public int NbVisites { get; set; }
    }
}