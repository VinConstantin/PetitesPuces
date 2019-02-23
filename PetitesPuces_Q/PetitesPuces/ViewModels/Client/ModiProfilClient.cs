using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace PetitesPuces.Models
{
    public class ModiProfilClient
    {
        [DisplayName("Nom de famille")]
        [RegularExpression("^([a-zA-Z0-9 \\.\\&\'\\-]+)$", ErrorMessage = "Votre format de nom est incorrect.")]
        [StringLength(50, ErrorMessage = "Le champs du nom doit avoir un maximum de 50 caractères.")]
        public string Nom { get; set; }

       
       
        [DisplayName("Prénom")]
        [RegularExpression("^([a-zA-Z0-9 \\.\\&\'\\-]+)$", ErrorMessage = "Votre format de prénom est incorrect.")]
        [StringLength(50, ErrorMessage = "Le champs du prénom doit avoir un maximum de 50 caractères.")]
        public string Prenom { get; set; }
        
        [DisplayName("Adresse")]
        [RegularExpression("^(\\d+\\s[A-z]+([ -]?[A-z]+)+?)$", ErrorMessage = "Votre format d'adresse est incorrect. Ex: 999 lavoie rue.")]
        [StringLength(50, ErrorMessage = "Le champs de la rue doit avoir un maximum de 50 caractères.")]
        public string Rue { get; set; }
        
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs de la ville a un format invalide.")]
        [StringLength(50, ErrorMessage = "Le champs de la ville doit avoir un maximum de 50 caractères.")]
        public string Ville { get;  set;}
       
        public string Province { get;  set;}
        
        
        
        [RegularExpression("^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$", ErrorMessage = "Le champs du code postal est invalide. Ex: A9A 9A9 / A9A9A9 / A9A-9A9")]
        [StringLength(7, ErrorMessage = "Le champs du code postal doit avoir un maximum de 7 caractères.")]
        public string CodePostal { get;  set;}
        
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs du pays a un format invalide")]
        [StringLength(10, ErrorMessage = "Le champs du pays doit avoir un maximum de 10 caractères.")]
        public string Pays { get;  set;}
        
        [DisplayName("Numéro téléphone 1")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format du téléphone 1 est invalide! Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 1 doit avoir un maximum de 20 caractères.")]
        public string Tel1 { get;  set;}
        
        [DisplayName("Numéro téléphone 2")]
        
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format du téléphone 2 est invalide! Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 2 doit avoir un maximum de 20 caractères.")]
        public string Tel2 { get;  set;}

        [DisplayName("Adresse de courriel")]
        public string AdresseCourriel { get; set; }

    }
}