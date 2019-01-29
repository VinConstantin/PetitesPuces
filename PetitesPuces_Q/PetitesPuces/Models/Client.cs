using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Client: IUtilisateur
    {
        [DisplayName("No Client")]
        public int NoClient { get; set; }

        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }

        public string MotDePasse { get; }

        [DisplayName("Nom")]
        public string Nom { get; set; }

        [DisplayName("Prénom")]
        public string Prenom { get; set; }

        [DisplayName("Date de dernière connexion")]
        public DateTime DateDerniereConnexion { get; set; }

        public string Rue { get; }
        public string Ville { get; }
        public string Province { get; }
        public string CodePostal { get; }
        public string Pays { get; }
        public DateTime? DateCreation { get; }
        public DateTime DateMAJ { get; }

        public string Role
        {
            get { return "Client"; }
        }
    }
}