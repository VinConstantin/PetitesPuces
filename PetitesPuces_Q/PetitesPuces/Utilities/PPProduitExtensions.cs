using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using PetitesPuces.Models;

namespace PetitesPuces.Utilities
{
    public static class PPProduitExtensions
    {
        public static decimal GetPrixCourant(this PPProduit produit)
        {
            return (decimal)((produit.DateVente >= DateTime.Now) ? produit.PrixVente : produit.PrixDemande);
        }
        
    }
}