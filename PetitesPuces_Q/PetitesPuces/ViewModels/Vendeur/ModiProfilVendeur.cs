using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class ModiProfilVendeur
    {
        [Required(ErrorMessage = "Veuillez entrer votre nom d'affaires!")]
        [DisplayName("Nom d'affaires")]
        [StringLength(50, ErrorMessage = "Le champs du nom d'affaires doit avoir un maximum de 50 caractères.")]
        public string NomAffaires { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre nom de famille")]
        [DisplayName("Nom de famille")]
        [RegularExpression(@"^([a-zA-Z0-9 \.\&\'\-]+)$", ErrorMessage = "Votre format de nom est incorrect.")]
        [StringLength(50, ErrorMessage = "Le champs du nom doit avoir un maximum de 50 caractères.")]
        public string Nom { get; set; }


        [Required(ErrorMessage = "Veuillez entrer votre prénom")]
        [DisplayName("Prénom")]
        [RegularExpression(@"^([a-zA-Z0-9 \.\&\'\-]+)$", ErrorMessage = "Votre format de prénom est incorrect.")]
        [StringLength(50, ErrorMessage = "Le champs du prénom doit avoir un maximum de 50 caractères.")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre rue")]
        [StringLength(50, ErrorMessage = "Le champs de la rue doit avoir un maximum de 50 caractères.")]
        [RegularExpression(@"^(\d+\s[A-z]+([ -]?[A-z]+)+?)$", ErrorMessage =
            "Votre format d'adresse est incorrect. Ex: 999 lavoie rue.")]
        [DisplayName("Adresse")]
        public string Rue { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre ville")]
        [RegularExpression(@"^[a-zA-Z]+(?:[\s-][a-zA-Z]+)*$", ErrorMessage = "Votre format de ville est incorrect.")]
        [StringLength(50, ErrorMessage = "Le champs de la ville doit avoir un maximum de 50 caractères.")]
        public string Ville { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre province")]
        [DisplayName("Province")]
        public string Province { get; set; }


        [Required(ErrorMessage = "Veuillez entrer votre code postal")]
        [RegularExpression("^[A-Za-z]\\d[A-Za-z][ -]?\\d[A-Za-z]\\d$", ErrorMessage =
            "Votre format du code postal est invalide.")]
        [StringLength(7, ErrorMessage = "Le champs du code postal doit avoir un maximum de 7 caractères.")]
        public string CodePostal { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre pays")]
        [RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage =
            "Le champs du pays a un format invalide")]
        [StringLength(10, ErrorMessage = "Le champs du pays doit avoir un maximum de 10 caractères.")]
        public string Pays { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre numéro de téléphone 1")]
        [DisplayName("Numéro téléphone 1")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage =
            "Le champs de la téléphone 1 a un format invalide. Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 1 doit avoir un maximum de 20 caractères.")]
        public string Tel1 { get; set; }


        [DisplayName("Numéro téléphone 2  (facultatif)")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage =
            "Le champs de la téléphone 2 a un format invalide. Ex:9999999999 / (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 2 doit avoir un maximum de 20 caractères.")]
        public string Tel2 { get; set; }


        [Range(0, 10000, ErrorMessage = "Le champs du poids MAX doit être entre 0 à 10000 kg")]
        [Required(ErrorMessage = "Veuillez entrer le poids max pour la livraison!")]
        [DisplayName("Poids max (KG)")]
        public int PoidsMaxLivraison { get; set; }

        [Range(0, 10000, ErrorMessage = "Le champs du prix minimum doit être un chiffre entre 0 à 10000 $")]
        [Required(ErrorMessage = "Veuillez entrer un prix minimum pour avoir une livraison graduite!")]
        [DisplayName("Prix minimum livraison graduite ($)")]
        public int LivraisonGratuite { get; set; }

        [Required(ErrorMessage = "Veuillez cocher si vous voulez que les clients payent les taxes (TPS et TVQ")]
        [DisplayName("Les clients paieront les taxes TPS et TVQ?")]
        public Boolean Taxes { get; set; }


        public string configuration { get; set; }
    }
}