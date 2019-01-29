using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class InactiviteViewModel
    {
        public List<Models.Vendeur> VendeursInactifs { get; set; }
        public List<Client> ClientsInactifs { get; set; }
        public List<IPersonne> UtilsRecherche { get; set; }
    }
}