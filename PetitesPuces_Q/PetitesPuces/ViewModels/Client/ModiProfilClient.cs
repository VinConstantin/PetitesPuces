using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace PetitesPuces.Models
{
    public class ModiProfilClient
    {
        [DisplayName("Nom de famille")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs nom a un format invalide.")]
        public string Nom { get; set; }

       
       
        [DisplayName("Prénom")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs prénom a un format invalide.")]
        public string Prenom { get; set; }
        
        [StringLength(100, ErrorMessage = "La rue doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Rue { get; set; }
        
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs ville a un format invalide.")]
        public string Ville { get;  set;}
       
        public string Province { get;  set;}
        
        
        
        [RegularExpression("^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$", ErrorMessage = "Le champs code postal est invalide. Ex: A9A 9A9 / A9A9A9 / A9A-9A9")]
        public string CodePostal { get;  set;}
        
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs pays a un format invalide")]
        public string Pays { get;  set;}
        
        [DisplayName("Numéro téléphone 1")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:9999999999/  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        public string Tel1 { get;  set;}
        
        [DisplayName("Numéro téléphone 2")]
        
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:9999999999/  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        public string Tel2 { get;  set;}


    }
}