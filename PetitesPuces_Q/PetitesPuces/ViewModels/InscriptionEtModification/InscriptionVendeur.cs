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
        
        [DisplayName("Adresse courriel")]
        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        public string AdresseEmail
        {
            get { return Vendeur.AdresseEmail; }
        }
        
        [DisplayName("Mot de passe")]
        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        public string MotDePasse
        {
            get { return Vendeur.MotDePasse; }
        }

      
        [Required(ErrorMessage = "Veuillez rentrer encore votre adresse courriel!")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier.")]
        [DisplayName("Confirmation courriel")]
        public string confirmationCourriel { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer encore une fois votre mot de passe!")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation mot de passe")]
        public string ConfirmationMDP { get; set; }
    }
}