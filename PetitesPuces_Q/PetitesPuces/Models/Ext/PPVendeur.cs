using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public partial class PPVendeur : IUtilisateur
    {
        private static readonly string ROLE = "vendeur";

        public string Role
        {
            get { return ROLE; }
        }
    }
}