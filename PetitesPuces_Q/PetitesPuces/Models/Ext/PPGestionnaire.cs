using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPVendeur_Validation))]
    public partial class PPGestionnaire: IUtilisateur
    {
        public string Role
        {
            get { return RolesUtil.ADMIN; }
        }

        public DateTime DateDerniereActivite
        {
            get
            {
                return DateTime.Today;
            }
        }

        public long No
        {
            get { return NoGestionnaire; }
        }
    }
    
    public class PPGestionnaireValidation
    {
        [DisplayName("No Gestionnaire")]
        public int NoGestionnaire { get; set; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        [DisplayName("Adresse Courriel")]
        public string AdresseEmail { get; set; }

      

        [Required(ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage = "Votre format de mot de passe est incorrect. Il doit avoir minimum 8 caractères et inclure au moins une majuscule.")]
        [DisplayName("Mot de passe")]
        public string MotDePasse { get; set; }


     

        [Required(ErrorMessage = "Veuillez rentrer votre nom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Nom")]
        public string Nom { get; set; }


        [Required(ErrorMessage = "Veuillez rentrer votre prénom!")]
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        [DisplayName("Prénom")]
        public string Prenom { get; set; }
    }
}