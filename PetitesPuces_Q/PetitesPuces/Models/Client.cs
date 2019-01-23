using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Client
    {
        public int NoClient { get; set; }
        public string AdresseEmail { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateDerniereConnexion { get; set; }
    }
}