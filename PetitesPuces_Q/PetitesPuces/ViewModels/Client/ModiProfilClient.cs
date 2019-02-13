using System.ComponentModel.DataAnnotations;

namespace PetitesPuces.Models
{
    public class ModiProfilClient
    {
        
        public string Nom { get; set; }


      
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Prenom { get; set; }
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Rue { get; set; }
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Ville { get;  set;}
       
        public string Province { get;  set;}
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string CodePostal { get;  set;}
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 2)]
        public string Pays { get;  set;}
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 10)]
        public string Tel1 { get;  set;}
        
        [StringLength(100, ErrorMessage = "Le {0} doit avoir au moins 2 caractères.", MinimumLength = 10)]
        public string Tel2 { get;  set;}


    }
}