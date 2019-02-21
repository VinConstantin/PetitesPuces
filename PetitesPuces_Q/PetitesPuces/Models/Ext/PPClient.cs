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
        
        [Required(ErrorMessage = "Veuillez entrer votre adresse de courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage =
            "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse de courriel")]
        public string AdresseEmail { get; set; }


        [Required(ErrorMessage = "Veuillez entrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Le champ mot de passe doit avoir un maximum de 50 caractères.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }

       
        [DisplayName("Nom")] public string Nom { get; set; }

     
        [DisplayName("Prénom")] public string Prenom { get; set; }

       
        [DisplayName("Date de dernière connexion")]
        public DateTime DateDerniereConnexion { get; set; }

     
        public string Rue { get; set; }
        
     
        public string Ville { get; set; }

        public string Province { get; set; }
        
      
        public string CodePostal { get; set; }
        
     
        public string Pays { get; set; }
        
     
        public string Tel1 { get; set; }
        

        public string Tel2 { get; set; }


     
        public DateTime? DateCreation { get; set; }
        
  
        public DateTime DateDerniereActivite { get; set; }
        
  
        public DateTime DateMAJ { get; set; }

        public string Role
        {
            get { return "Client"; }
        }
    }
}