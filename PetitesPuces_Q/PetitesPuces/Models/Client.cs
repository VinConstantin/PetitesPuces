using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Client
    {
        [DisplayName("No Client")]
        public int NoClient { get; set; }

        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }

        [DisplayName("Nom")]
        public string Nom { get; set; }

        [DisplayName("Prénom")]
        public string Prenom { get; set; }

        [DisplayName("Date de dernière connexion")]
        public DateTime DateDerniereConnexion { get; set; }
    }
}