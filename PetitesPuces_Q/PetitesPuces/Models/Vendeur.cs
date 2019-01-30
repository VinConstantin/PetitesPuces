using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PetitesPuces.Validations;

namespace PetitesPuces.Models
{
    public class Vendeur: IUtilisateur
    {
        [DisplayName("No Vendeur")]
        public int NoVendeur { get; set; }

        [Required (ErrorMessage = "Veuillez rentrer le nom d'affaire!")]
        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [ValidationCourriel]
        [DisplayName("Adresse courriel")]
        public string AdresseEmail { get; set; }
        public string MotDePasse { get; }

        [Required(ErrorMessage = "Veuillez rentrer encore votre adresse courriel!")]
        [DisplayName("Confirmation courriel")]
        public string confirmationCourriel { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre nom!")]
        [DisplayName("Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre prénom!")]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }


        [DisplayName("Date de la demande")]
        public DateTime? DateCreation { get; set; }

        public DateTime DateDerniereActivite { get; }

        public DateTime DateMAJ { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DisplayName("Mot de passe")]
        public string motDePasse { get; }

        [Required(ErrorMessage = "Veuillez rentrer encore une fois votre mot de passe!")]
        [DisplayName("confirmation mot de passe")]
        public string confirmationMDP { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre rue!")]
        [DisplayName("Rue")]
        public string Rue { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre ville!")]
        [DisplayName("Ville")]
        public string Ville { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre province!")]
        [DisplayName("Province")]
        public string Province { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre code postal!")]
        [DisplayName("Code Postal")]
        public string CodePostal { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre pays!")]
        [DisplayName("pays")]
        public string Pays { get; }

        [Range(0, 10000)]
        [Required(ErrorMessage = "Veuillez rentrer le poids max pour la livraison!")]
        [DisplayName("Poids max (KG)")]
        public string poidsMax { get; }

        [Range(0,10000)]
        [Required(ErrorMessage = "Veuillez rentrer un prix minimum pour une livraison graduite!")]
        [DisplayName("Prix minimum")]
        public int prixMinimum { get; }

       
        [DisplayName("Taxes TPS/TVQ")]
        public Boolean taxes { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre numéro de téléphone!")]
        [DisplayName("Téléphone No1")]
        public string tel1 { get; }

        [Required(ErrorMessage = "Veuillez rentrer un autre numéro de téléphone!")]
        [DisplayName("Téléphone No1")]
        public string tel2 { get; }

      

        public string Role
        {
            get { return "Vendeur"; }
        }
    }
}