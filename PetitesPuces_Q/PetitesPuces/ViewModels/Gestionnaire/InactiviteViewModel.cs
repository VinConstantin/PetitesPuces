using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class InactiviteViewModel
    {
        public List<Models.PPVendeur> VendeursInactifs { get; set; }
        public List<PPClient> ClientsInactifs { get; set; }
        public List<IUtilisateur> UtilsRecherche { get; set; }
    }
}