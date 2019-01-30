using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Client : IPersonne
    {

        [DisplayName("No Client")]
        public int NoClient { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DisplayName("Mot de passe")]
        public string motDePasse { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre nom!")]
        [DisplayName("Nom")]
        public string Nom { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre prénom!")]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe encore une autre fois!")]
        [DisplayName("Confirmation de mot de passe")]
        public string confirmationMDP { get; set; }

        [DisplayName("Date de dernière connexion")]
        public DateTime DateDerniereConnexion { get; set; }



        public string Rue { get; }
        public string Ville { get; }
        public string Province { get; }
        public string CodePostal { get; }
        public string Pays { get; }
        public DateTime DateCreation { get; }
        public DateTime DateMAJ { get; }

        public string Role
        {
            get { return "Client"; }
        }
    }
}