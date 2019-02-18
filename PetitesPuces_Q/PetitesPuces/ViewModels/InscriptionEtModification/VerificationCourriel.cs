using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class VerificationCourriel
    {

        
        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        public string AdresseEmail { get; set; }
    }
}