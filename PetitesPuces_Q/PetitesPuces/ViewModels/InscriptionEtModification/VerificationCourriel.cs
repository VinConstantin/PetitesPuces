using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.ViewModels
{
    public class VerificationCourriel
    {

        [DisplayName("Adresse de courriel")]
        [Required(ErrorMessage = "Veuillez entrer votre adresse de courriel!")]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Votre format de courriel est incorrect.")]
        public string AdresseEmail { get; set; }
    }
}