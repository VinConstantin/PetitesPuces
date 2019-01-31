using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models.Ext
{
    public class PPGestionnaire
    {
        [DisplayName("No Gestionnaire")]
        public int NoGestionnaire { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier")]
        [DisplayName("Confirmation Courriel")]
        public string ConfirmationCourriel { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe encore une autre fois!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation de mot de passe")]
        public string confirmationMDP { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre nom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Nom")]
        public string Nom { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre prénom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }
    }
}