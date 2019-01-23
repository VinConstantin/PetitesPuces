using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetitesPuces.Models
{
    public interface IPersonne
    {
        string Role { get; }
        string AdresseEmail { get; }
        string Nom { get; }
        string Prenom { get; }
        string Rue { get; }
        string Ville { get; }
        string Province { get; }
        string CodePostal { get; }
        string Pays { get; }
        DateTime DateCreation { get; }
        DateTime DateMAJ { get; }
    }
}
