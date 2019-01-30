using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetitesPuces.Models
{
    [MetadataType(typeof(PPClient_Validation))]
    public partial class PPClient : IUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();
        private static readonly string ROLE = "client";
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
            return (from paniers
                            in ctxt.PPArticlesEnPaniers
                    where paniers.NoClient == NoClient
                    select paniers.DateCreation.GetValueOrDefault())
                    .Concat(
                        from commande
                            in ctxt.PPCommandes
                        where commande.NoClient == NoClient
                        select commande.DateCommande.GetValueOrDefault()).AsEnumerable().ToList().DefaultIfEmpty(DateTime.MinValue).Max();
        }
    }

    [Bind()]
    public class PPClient_Validation
    {

    }
}