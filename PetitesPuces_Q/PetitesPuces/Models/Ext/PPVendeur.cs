using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPVendeur_Validation))]
    public partial class PPVendeur : IUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctx = new BDPetitesPucesDataContext();
        private DateTime? _dateDerniereActivite;

        public string Role
        {
            get { return RolesUtil.VEND; }
        }
        public string DisplayName
        {
            get { return NomAffaires; }
        }
        public long No
        {
            get { return NoVendeur; }
        }

        public DateTime DateDerniereActivite
        {
            get
            {
                if (!_dateDerniereActivite.HasValue)
                {
                    _dateDerniereActivite = CalculerDerniereActivite();
                }

                return _dateDerniereActivite.Value;
            }
        }

        public DateTime CalculerDerniereActivite()
        {
            return
                (from produit
                        in ctx.PPProduits
                 where produit.NoVendeur == NoVendeur
                 select produit.DateCreation.GetValueOrDefault((DateTime) SqlDateTime.MinValue))
                .Concat(
                    from commande
                        in ctx.PPCommandes
                    where commande.NoVendeur == NoVendeur
                    select commande.DateCommande.GetValueOrDefault((DateTime) SqlDateTime.MinValue)
                ).AsEnumerable().DefaultIfEmpty((DateTime) SqlDateTime.MinValue).Max();
        }
    }

    [Bind]
    public class PPVendeur_Validation
    {
        [DisplayName("No Vendeur")]
        public int NoVendeur { get; set; }
        
      
        [Required(ErrorMessage = "Veuillez entrer le nom d'affaires!")]
        [StringLength(50, ErrorMessage = "Le champs du nom d'affaires doit avoir un maximum de 50 caractères.")]
        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre adresse de courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse de courriel")]
        public string AdresseEmail { get; set; }
     
      
        [Required(ErrorMessage = "Veuillez entrer votre nom!")]
        [StringLength(50, ErrorMessage = "Le champs du nom doit avoir un maximum de 50 caractères.")]
        [RegularExpression(@"^([a-zA-Z0-9 \.\&\'\-]+)$", ErrorMessage = "Votre format de nom est incorrect.")]
        [DisplayName("Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Veuillez entrer votre prénom!")]
        [StringLength(50, ErrorMessage = "Le champs du prénom doit avoir un maximum de 50 caractères.")]
        [RegularExpression(@"^([a-zA-Z0-9 \.\&\'\-]+)$", ErrorMessage = "Votre format de prénom est incorrect.")]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }

     
       
        public DateTime? DateCreation { get; set; }

        public DateTime DateDerniereActivite { get;  set;}

        public DateTime DateMAJ { get;  set;}

        [Required(ErrorMessage = "Veuillez entrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Le champ mot de passe doit avoir un maximum de 50 caractères.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }

      
      
        [Required(ErrorMessage = "Veuillez entrer votre rue!")]
        [StringLength(50, ErrorMessage = "Le champs de la rue doit avoir un maximum de 50 caractères.")]
        [RegularExpression(@"^(\d+\s[A-z]+([ -]?[A-z]+)+?)$", ErrorMessage = "Votre format d'adresse est incorrect. Ex: 999 lavoie rue.")]
        [DisplayName("Adresse")]
        public string Rue { get; set; }

      
        [Required(ErrorMessage = "Veuillez entrer votre ville!")]
        [StringLength(50, ErrorMessage = "Le champs de la ville doit avoir un maximum de 50 caractères.")]
        [RegularExpression(@"^[a-zA-Z]+(?:[\s-][a-zA-Z]+)*$", ErrorMessage = "Votre format de ville est incorrect." )]
        [DisplayName("Ville")]
        public string Ville { get; set; }

        
        [Required(ErrorMessage = "Veuillez entrer votre province!")]
        [DisplayName("Province")]
        public string Province { get; set; }

     
        [Required(ErrorMessage = "Veuillez entrer votre code postal!")]
        [DisplayName("Code Postal")]
        [RegularExpression(@"^[\w][\d][\w][- ]?[\d][\w][\d]$", ErrorMessage = "Votre format de code postal est incorrect.")]
        [StringLength(7, ErrorMessage = "Le champs du code postal doit avoir un maximum de 7 caractères.")]
        public string CodePostal { get;  set;}

      
        [Required(ErrorMessage = "Veuillez entrer votre pays!")]
        [StringLength(10, ErrorMessage = "Le champs du pays doit avoir un maximum de 10 caractères.")]
        [DisplayName("Pays")]
        public string Pays { get;  set;}


        [Range(0, 10000, ErrorMessage = "S.V.P de entrer un chiffre entre 0 à 10000 kg")]
        [Required(ErrorMessage = "Veuillez entrer le poids max pour la livraison!")]
        [DisplayName("Poids max (KG)")]
        public int PoidsMaxLivraison { get;  set;}

       
        [Range(0, 10000, ErrorMessage = "S.V.P de entrer un chiffre entre 0 à 10000 $")]
        [Required(ErrorMessage = "Veuillez entrer un prix au moins pour une livraison graduite!")]
        [DisplayName("Prix minimum livraison graduite ($)")]
        public int LivraisonGratuite { get;  set;}

        [Required(ErrorMessage = "Veuillez cocher si vous voulez que les clients payent les taxes (TPS et TVQ")]
        [DisplayName("Les clients paieront les taxes TPS et TVQ ?")]
        public bool Taxes { get;  set;}

 
        [Required(ErrorMessage = "Veuillez entrer votre numéro de téléphone!")]
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 1 doit avoir un maximum de 20 caractères.")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:9999999999/  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [DisplayName("Téléphone No1")]
        public string Tel1 { get;  set;}

     
       
        [StringLength(20, ErrorMessage = "Le champs de la téléphone 2 doit avoir un maximum de 20 caractères.")]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]?\\d{3}[\\s.-]?\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:9999999999/  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [DisplayName("Téléphone No2 (facultatif)")]
        public string Tel2 { get;  set;}

        public string Configuration { get;set; }
        
        public string Role
        {
            get { return "Vendeur"; }
        }
    }
}