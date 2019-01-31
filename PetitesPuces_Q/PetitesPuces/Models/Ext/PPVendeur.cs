using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPVendeur_Validation))]
    public partial class PPVendeur : IUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctx = new BDPetitesPucesDataContext();
        private static readonly string ROLE = "vendeur";
        private DateTime? _dateDerniereActivite;

        public string Role
        {
            get { return ROLE; }
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
                 select produit.DateCreation.GetValueOrDefault())
                .Concat(
                    from commande
                        in ctx.PPCommandes
                    where commande.NoVendeur == NoVendeur
                    select commande.DateCommande.GetValueOrDefault()
                ).Max();
        }
    }

    [Bind]
    public class PPVendeur_Validation
    {
        [DisplayName("No Vendeur")]
        public int NoVendeur { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer le nom d'affaire!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Nom d'affaires")]
        public string NomAffaires { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse courriel")]
        public string AdresseEmail { get; set; }

       /* [Required(ErrorMessage = "Veuillez rentrer encore votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [System.ComponentModel.DataAnnotations.Compare("AdresseEmail", ErrorMessage = "Le second courriel ne corespond pas au premier.")]
        [DisplayName("Confirmation courriel")]
        public string confirmationCourriel { get;  set;}
        */
        [Required(ErrorMessage = "Veuillez rentrer votre nom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre prénom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }


        [DisplayName("Date de la demande")]
        public DateTime? DateCreation { get; set; }

        public DateTime DateDerniereActivite { get;  set;}

        public DateTime DateMAJ { get;  set;}



        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 8 caractères.", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }

       /* [Required(ErrorMessage = "Veuillez rentrer encore une fois votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 8 caractères.", MinimumLength = 8)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [System.ComponentModel.DataAnnotations.Compare("MotDePasse", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        [DisplayName("Confirmation mot de passe")]
        public string confirmationMDP { get; set; }
        */
        [Required(ErrorMessage = "Veuillez rentrer votre rue!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Rue")]
        public string Rue { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre ville!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Ville")]
        public string Ville { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre province!")]
        [DisplayName("Province")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre code postal!")]
        [DisplayName("Code Postal")]
        public string CodePostal { get;  set;}

        [Required(ErrorMessage = "Veuillez rentrer votre pays!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Pays")]
        public string Pays { get;  set;}

        [Range(0, 10000, ErrorMessage = "S.V.P de rentrer un chiffre entre 0 à 10000 kg")]
        [Required(ErrorMessage = "Veuillez rentrer le poids max pour la livraison!")]
        [DisplayName("Poids max (KG)")]
        public int PoidsMaxLivraison { get;  set;}

        [Range(0, 10000, ErrorMessage = "S.V.P de rentrer un chiffre entre 0 à 10000 $")]
        [Required(ErrorMessage = "Veuillez rentrer un prix au moins pour une livraison graduite!")]
        [DisplayName("Prix minimum")]
        public int LivraisonGratuite { get;  set;}


        [DisplayName("Taxes TPS/TVQ")]
        public Boolean Taxes { get;  set;}

        [Required(ErrorMessage = "Veuillez rentrer votre numéro de téléphone!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 10 caractères.", MinimumLength = 10)]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [DisplayName("Téléphone No1")]
        public string Tel1 { get;  set;}

        [Required(ErrorMessage = "Veuillez rentrer un autre numéro de téléphone!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 10 caractères.", MinimumLength = 10)]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$", ErrorMessage = "Le format de téléphone est invalide! Ex:  (999) 999-9999 / 999-999-9999 / 999 999 9999 / 999.999.9999 / +99 (999) 999-9999")]
        [DisplayName("Téléphone No2")]
        public string Tel2 { get;  set;}


        public string Role
        {
            get { return "Vendeur"; }
        }
    }
}