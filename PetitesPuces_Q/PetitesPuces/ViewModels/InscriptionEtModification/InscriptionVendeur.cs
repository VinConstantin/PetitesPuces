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
        
        [DisplayName("Adresse de courriel")]
        [Required(ErrorMessage = "Veuillez entrer votre adresse de courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        public string AdresseEmail
        {
            get { return Vendeur.AdresseEmail; }
        }
        
        [DisplayName("Mot de passe")]
        [Required(ErrorMessage = "Veuillez entrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Le champ mot de passe doit avoir un maximum de 50 caractères.")] public string MotDePasse
        {
            get { return Vendeur.MotDePasse; }
        }

      
        [Required(ErrorMessage = "Veuillez entrer encore votre adresse de courriel!")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier.")]
        [DisplayName("Confirmation de courriel")]
        public string confirmationCourriel { get; set; }

        [Required(ErrorMessage = "Veuillez entrer encore une fois votre mot de passe!")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation de mot de passe")]
        public string ConfirmationMDP { get; set; }
        
      
    }
}