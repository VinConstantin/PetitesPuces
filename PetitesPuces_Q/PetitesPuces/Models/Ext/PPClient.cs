using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPClient_Validation))]
    public partial class PPClient : IUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();
        private DateTime? _dateDerniereActivite;

        public string Role
        {
            get { return RolesUtil.CLIENT; }
        }

        public string DisplayName
        {
            get { 
                if(string.IsNullOrEmpty(Nom))
                {
                    return AdresseEmail.Split('@')[0];
                } return Nom + ", " + Prenom; 
            }
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

        public long No
        {
            get { return NoClient; }
        }

        public DateTime CalculerDerniereActivite()
        {
            return (from paniers
                        in ctxt.PPArticlesEnPaniers
                    where paniers.NoClient == NoClient
                    select paniers.DateCreation.GetValueOrDefault())
                .Concat(
                    from commande
                        in ctxt.PPCommandes
                    where commande.NoClient == NoClient
                    select commande.DateCommande.GetValueOrDefault()).AsEnumerable().ToList()
                .DefaultIfEmpty(DateTime.MinValue).Max();
        }
    }

    [Bind()]
    public class PPClient_Validation
    {
        [DisplayName("No Client")] public int NoClient { get; set; }
        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage =
            "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage =
            "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }

        // [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DataType(DataType.Text)]
        [DisplayName("Nom")] public string Nom { get; set; }

        [DataType(DataType.Text)]
        //     [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Prénom")] public string Prenom { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date de dernière connexion")]
        public DateTime DateDerniereConnexion { get; set; }

        [DataType(DataType.Text)]
        public string Rue { get; set; }
        
        [DataType(DataType.Text)]
        public string Ville { get; set; }

        public string Province { get; set; }
        
        [DataType(DataType.PostalCode)]
        public string CodePostal { get; set; }
        
        [DataType(DataType.Text)]
        public string Pays { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string Tel1 { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string Tel2 { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DateCreation { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DateDerniereActivite { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DateMAJ { get; set; }

        public string Role
        {
            get { return "Client"; }
        }
    }
}