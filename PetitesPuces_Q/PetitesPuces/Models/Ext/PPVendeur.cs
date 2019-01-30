using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPVendeur_Validation))]
    public partial class PPVendeur : IUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctx = new BDPetitesPucesDataContext();
        private static readonly string ROLE = "vendeur";
        private DateTime? _dateDerniereActivite;

        public string Role
        {
            get { return ROLE; }
        }

        public DateTime DateDerniereActivite
        {
            get
            {
                if (!_dateDerniereActivite.HasValue)
                {
                    _dateDerniereActivite = CalculerDerniereActivite();
                }

                return _dateDerniereActivite.Value;
            }
        }

        public DateTime CalculerDerniereActivite()
        {
            return
                (from produit
                        in ctx.PPProduits
                    where produit.NoVendeur == NoVendeur
                    select produit.DateCreation.GetValueOrDefault())
                .Concat(
                    from commande
                        in ctx.PPCommandes
                    where commande.NoVendeur == NoVendeur
                    select commande.DateCommande.GetValueOrDefault()
                ).Max();
        }
    }

    [Bind]
    public class PPVendeur_Validation
    {

    }
}