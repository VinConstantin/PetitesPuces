using System;
using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class PageStatsViewModel
    {
        public List<SerializablePPClient> Clients { get; set;} 
        public List<SerializablePPVendeur> Vendeurs { get; set;} 
    }

    public static class TypesClient
    {
        public const string ACTIF = "Actifs";
        public const string POTEN = "Potentiels";
        public const string VISIT = "Visiteurs";
    }
}