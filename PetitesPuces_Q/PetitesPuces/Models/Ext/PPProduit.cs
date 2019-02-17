using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using PetitesPuces.Securite;

namespace PetitesPuces.Models
{
    public partial class PPProduit
    {
        private BDPetitesPucesDataContext ctxt;

        public bool EstEnRabais
        {
            get { return DateVente >= DateTime.Now; }
        }
    }
}