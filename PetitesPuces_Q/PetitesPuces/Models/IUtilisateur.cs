using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Models
{
    public interface IUtilisateur
    {
        string Role { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        [DisplayName ("Adresse courriel")]
        string AdresseEmail { get; }

        string Nom { get; }

        string Prenom { get; }

        string DisplayName { get; }

        [DisplayName("Mot de passe")]
        [Required (ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        string MotDePasse { get;  }

      
        DateTime DateDerniereActivite { get; }
        
        long No { get;  }
    }
    
    public enum StatutCompte : short
    {
        INACTIF = 0,
        ACTIF = 1,
        DESACTIVE = 2,
    }
}
