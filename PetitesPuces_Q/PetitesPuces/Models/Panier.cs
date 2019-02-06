using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public class Panier
    {
        [DisplayName("Date de création")]
        public DateTime DateCreation { get; set; }
        
        public PPClient Client { get; set; }
        public PPVendeur Vendeur { get; set; }
        public List<PPArticlesEnPanier> Articles { get; set; }

        public decimal getPrixTotal()
        {
            return (decimal) Articles.Sum(a => a.PPProduit.PrixVente * a.NbItems);
        }

        public int GetNbItems()
        {
            return (int)Articles.Sum(a => a.NbItems);
        }
    }
}