using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.Models
{
    public enum Province
    {
        [Display(Name = "Québec")]
        QC,

        [Display(Name = "Ontario")]
        ON,
        
        [Display(Name = "Nouveau-Brunswick")]
        NB
    }
}