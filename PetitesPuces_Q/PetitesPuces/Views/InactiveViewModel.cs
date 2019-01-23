using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class InactiviteViewModel
    {
        public List<Vendeur> VendeursInactifs { get; set; }
        public List<Client> ClientsInactifs { get; set; }
    }
}