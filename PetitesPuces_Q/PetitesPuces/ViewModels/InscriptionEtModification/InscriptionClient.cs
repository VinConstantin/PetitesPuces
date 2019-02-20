using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels
{
    public class InscriptionClient
    {
        public PPClient client { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        public string AdresseEmail
        {
            get { return client.AdresseEmail; }
        }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage =  "Votre format de mot de passe est invalide. Il doit avoir un minimum de 8 caractères et inclure au moins une majuscule,un minuscule et un chiffre.")]
        public string MotDePasse
        {
            get { return client.MotDePasse; }
        }

       
        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier")]
        [DisplayName("Confirmation Courriel")]
        public string ConfirmationCourriel { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe encore une autre fois!")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation de mot de passe")]
        public string ConfirmationMDP { get; set; }
    }
}