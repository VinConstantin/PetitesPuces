using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Courriel
{
    public class DestinataireViewModel
    {
        public List<PPClient> Clients { get; set; }
        public List<PPVendeur> Vendeurs { get; set; }
    }
}