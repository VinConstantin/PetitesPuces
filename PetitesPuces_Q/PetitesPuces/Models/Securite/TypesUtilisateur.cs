using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PetitesPuces.Models.Ext
{
    public enum TypesUtilisateur
    {
        CLIENT = 0,
        VENDEUR = 1,
        GESTIONNAIRE = 2,
    }

    public class RoleVendeur : IdentityRole
    {
        public TypesUtilisateur ID
        {
            get
            {
                return TypesUtilisateur.VENDEUR;
            }
        }
    }
}