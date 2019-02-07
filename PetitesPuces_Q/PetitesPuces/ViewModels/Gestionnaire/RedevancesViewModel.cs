using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class RedevancesViewModel
    {
        public List<Redevance> Redevances { get; set; }
        public List<PPVendeur> Vendeurs { get; set; }
    }
}