using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.Models
{
    public partial class PPClient : IUtilisateur
    {
        private static readonly string ROLE = "client";

        public string Role
        {
            get { return ROLE; }
        }
    }
}