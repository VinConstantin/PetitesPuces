using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Models
{
    public interface IPersonne
    {
        string Role { get; }

        [Required(ErrorMessage = "Veuillez rentrer votre adresse courriel!")]
        string AdresseEmail { get; }

        string Nom { get; }

        string Prenom { get; }

        [Required (ErrorMessage = "Veuillez rentrer votre mot de passe!")]
        string motDePasse { get;  }

        string Rue { get; }
        string Ville { get; }
        string Province { get; }
        string CodePostal { get; }
        string Pays { get; }
        DateTime DateCreation { get; }
        DateTime DateMAJ { get; }
    }
}
