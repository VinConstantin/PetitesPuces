using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class StatsViewModel
    {
        public int TotalVendeurs { get; set; }
        public Dictionary<TimeSpan, int> NouveauxVendeurs { get; set; }
        public Dictionary<TimeSpan, int> ClientsActifs { get; set; }
        public Dictionary<TimeSpan, int> ClientsPotentiels { get; set; }
        public Dictionary<TimeSpan, int> ClientsVisiteurs { get; set; }
        public Dictionary<Client, int> NombreConnexionClients { get; set; }
        public List<Client> DernieresConnexions { get; set; }
    }
}