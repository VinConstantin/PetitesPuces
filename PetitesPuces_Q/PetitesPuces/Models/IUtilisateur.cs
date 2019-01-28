using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace PetitesPuces.Models
{
    public interface IUtilisateur: IUser<int>
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
