using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.Models
{
    public enum Tri
    {
        [Display(Name = "Numéro de produit")]
        Numero,

        [Display(Name = "Catégorie")]
        Categorie,
        
        [Display(Name = "Nom")]
        [Description("Nom")]
        Nom,
        
        [Display(Name = "Date de parution")]
        Date
    }
}