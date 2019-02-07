using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels
{
    public class InscriptionVendeur
    {
        public PPVendeur Vendeur { get; set; }

        public string AdresseEmail
        {
            get { return Vendeur.AdresseEmail; }
        }

        public string MotDePasse
        {
            get { return Vendeur.MotDePasse; }
        }

        [Required(ErrorMessage = "Veuillez rentrer encore votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier.")]
        [DisplayName("Confirmation courriel")]
        public string confirmationCourriel { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer encore une fois votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 8 caractères.", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation mot de passe")]
        public string ConfirmationMDP { get; set; }
    }
}