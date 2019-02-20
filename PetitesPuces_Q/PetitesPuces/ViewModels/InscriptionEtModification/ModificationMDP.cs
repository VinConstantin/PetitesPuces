using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class ModificationMDP
    {
        [DisplayName("Ancien mot de passe")]
        [Required(ErrorMessage = "Vous devez rentrer l'ancien mot de passe")]
        [DataType(DataType.Password)]
        public string ancienMDP { get; set; }
        
        [DisplayName("Nouveau mot de passe")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,100}$", ErrorMessage =  "Votre format de mot de passe est invalide. Il doit avoir un minimum de 8 caractères et inclure au moins une majuscule,un minuscule et un chiffre.")]
        [Required(ErrorMessage = "Vous devez rentrer un nouveau mot de passe")]
        [DataType(DataType.Password)]
        public string motDePass { get; set; }
        
        [DisplayName("Confirmation mot de passe")]
        [Required(ErrorMessage = "Vous devez confirmer votre mot de passe")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("motDePass", ErrorMessage = "Le second mot de passe ne corespond pas au premier.")]
        public string confirmationMDP { get; set; }
    }
}