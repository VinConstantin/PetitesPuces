using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.Models
{
    public enum Tri
    {
        [Display(Name = "Numéro de produit")]
        [Description("Numéro de produit")]
        Numero,

        [Display(Name = "Catégorie")]
        [Description("Catégorie")]
        Categorie,
        
        [Display(Name = "Nom")]
        [Description("Nom")]
        Nom,
        
        [Display(Name = "Date de parution")]
        [Description("Date de parution")]
        Date
    }
}