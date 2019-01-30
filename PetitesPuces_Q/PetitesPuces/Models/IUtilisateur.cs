using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Models
{
    public interface IUtilisateur
    {
        string Role { get; }
        string AdresseEmail { get; }
        string MotDePasse { get; }
        string Nom { get; }
        string Prenom { get; }
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
