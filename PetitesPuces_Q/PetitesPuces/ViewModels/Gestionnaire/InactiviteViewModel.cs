using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class InactiviteViewModel
    {
        public List<Vendeur> VendeursInactifs { get; set; }
        public List<Models.Client> ClientsInactifs { get; set; }
    }
}