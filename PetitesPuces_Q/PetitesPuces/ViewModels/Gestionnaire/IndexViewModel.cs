using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class IndexViewModel
    {
        public Dictionary<string, int> NombreDemandesVendeur;
        public Dictionary<string, decimal> Redevances { get; set; }
        public Dictionary<string, int> UtilisateursInactifs { get; set; }
    }
}