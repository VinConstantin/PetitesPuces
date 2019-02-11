using System;
using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class SerializablePPClient
    {
        private PPClient client;
        
        public SerializablePPClient(PPClient client)
        {
            this.client = client;
        }
        
        public string Nom {get {return client.Nom;}}
        public string Prenom {get {return client.Prenom;}}
        public long NoClient {get {return client.NoClient;}}
        public string AdresseEmail {get {return client.AdresseEmail;}}
        public DateTime DateDerniereConnexion {get {return client.DateDerniereConnexion.GetValueOrDefault(DateTime.MinValue);}}
        public int NbConnexions {get {return client.NbConnexions.GetValueOrDefault(0);}}
        public Dictionary<long, StatsVendeurClient> TotalCommandesParVendeur { get; set; }
        public decimal TotalCommandes { get; set; }
        public string Role
        {
            get { return client.Role; }
        }
    }
}