using System;
using System.Collections.Generic;
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
        string AdresseEmail { get; }

        string Nom { get; }

        string Prenom { get; }

        [Required (ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        string MotDePasse { get;  }

        string Rue { get; }
        DateTime? DateCreation { get; }
        DateTime DateDerniereActivite { get; }
    }
    
    public enum StatutCompte : short
    {
        INACTIF = 0,
        ACTIF = 1,
    }
}
