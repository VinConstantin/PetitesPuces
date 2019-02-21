using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class ModificationMDP
    {
        [DisplayName("Ancien mot de passe")]
        [Required(ErrorMessage = "Vous devez entrer l'ancien mot de passe")]
        [DataType(DataType.Password)]
        public string ancienMDP { get; set; }
        
        [DisplayName("Nouveau mot de passe")]
        [Required(ErrorMessage = "Vous devez entrer un nouveau mot de passe")]
        [StringLength(0-50, ErrorMessage = "Le champ mot de passe doit avoir un maximum de 50 caractères.")]
        [DataType(DataType.Password)]
        public string motDePass { get; set; }
        
        [DisplayName("Confirmation mot de passe")]
        [Required(ErrorMessage = "Vous devez confirmer votre mot de passe")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("motDePass", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        public string confirmationMDP { get; set; }
    }
}