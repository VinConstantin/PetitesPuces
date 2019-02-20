using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class ModiProfilVendeur
    {
        [Required(ErrorMessage = "Veuillez entrer votre nom d'affaire!")]
        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre nom de famille")]
        [DisplayName("Nom de famille")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs du nom de famille a un format invalide")]
        public string Nom { get; set; }

       
        [Required(ErrorMessage = "Veuillez entrer votre prénom")]
        [DisplayName("Prénom")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs du prénom a un format invalide")]
        public string Prenom { get; set; }
        
        [Required(ErrorMessage = "Veuillez entrer votre rue")]
        [StringLength(100, ErrorMessage = "Le champs de la rue doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Rue { get; set; }
        
        [Required(ErrorMessage = "Veuillez entrer votre ville")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs de la ville a un format invalide")]
        public string Ville { get;  set;}
        
        [Required(ErrorMessage = "Veuillez entrer votre province")]
        public string Province { get;  set;}
        
        
        [Required(ErrorMessage = "Veuillez entrer votre code postal")]
        [RegularExpression("^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$", ErrorMessage = "Votre format du code postal est invalide.")]
       public string CodePostal { get;  set;}
        
       [Required(ErrorMessage = "Veuillez entrer votre pays")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Le champs du pays a un format invalide")]
        public string Pays { get;  set;}
        
        [Required(ErrorMessage = "Veuillez entrer votre numéro de téléphone 1")]
        [DisplayName("Numéro téléphone 1")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le champs de la téléphone 1 a un format invalide. Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        public string Tel1 { get;  set;}
        
        [Required(ErrorMessage = "Veuillez entrer votre numéro de téléphone 2")]
        [DisplayName("Numéro téléphone 2")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le champs de la téléphone 2 a un format invalide. Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        public string Tel2 { get;  set;}


        [Range(0, 10000, ErrorMessage = "Le champs du poids MAX doit être entre 0 à 10000 kg")]
        [Required(ErrorMessage = "Veuillez entrer le poids max pour la livraison!")]
        [DisplayName("Poids max (KG)")]
        public int PoidsMaxLivraison {     get;  set;}

        [Range(0, 10000, ErrorMessage = "Le champs du prix minimum doit être un chiffre entre 0 à 10000 $")]
        [Required(ErrorMessage = "Veuillez entrer un prix minimum pour avoir une livraison graduite!")]
        [DisplayName("Prix minimum livraison graduite ($)")]
        public int LivraisonGratuite { get;  set;}

        [Required(ErrorMessage = "Veuillez cocher si vous voulez payer les taxes (TPS et TVQ")]
        [DisplayName("Taxes TPS/TVQ")]
        public Boolean Taxes { get;  set;}
        
        
        public string configuration { get;  set;}
    }
}