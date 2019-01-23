using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class StatsViewModel
    {
        public Dictionary<TimeSpan, int> TotalVendeurs { get; set; }
        public Dictionary<TimeSpan, int> NouveauClients { get; set; }
    }
}