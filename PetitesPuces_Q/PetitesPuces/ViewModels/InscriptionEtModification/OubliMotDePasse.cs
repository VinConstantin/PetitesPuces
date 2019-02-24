using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class OubliMotDePasse
    {
        [DisplayName("Nouveau mot de passe")]
        [Required(ErrorMessage = "Vous devez entrer un nouveau mot de passe")]
        [StringLength(50, ErrorMessage = "Le champ mot de passe doit avoir un maximum de 50 caractères.")]
        [DataType(DataType.Password)]
        public string motDePass { get; set; }

        [DisplayName("Confirmation du mot de passe")]
        [Required(ErrorMessage = "Vous devez confirmer votre mot de passe")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("motDePass", ErrorMessage =
            "Le second mot de passe ne correspond pas au premier.")]
        public string confirmationMDP { get; set; }
        
        public string courriel { get; set; }
    }
}