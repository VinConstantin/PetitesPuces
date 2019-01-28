using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PetitesPuces.Models
{
    public partial class PPVendeur : IdentityUser
    {
        public IdentityRole Role { get { return "Vendeur" } }
    }
}