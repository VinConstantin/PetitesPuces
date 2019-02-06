using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class DetailsRedevances
    {
        public PPVendeur Vendeur { get; set; }
        public List<PPHistoriquePaiement> Paiements { get; set; }
    }
}