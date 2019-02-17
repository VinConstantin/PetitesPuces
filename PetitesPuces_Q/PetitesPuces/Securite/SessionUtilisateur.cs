using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using PetitesPuces.Models;

namespace PetitesPuces.Securite
{
    public static class SessionUtilisateur
    {
        private static readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();

        public static long? NoUtilisateur
        {
            get { return (long?) HttpContext.Current.Session["userId"]; }
        }

        public static IUtilisateur UtilisateurCourant
        {
            get
            {
                var userId = (long?)HttpContext.Current.Session["userId"];

                if (!userId.HasValue) return null;

                IUtilisateur infosUtil = GetAllUsersWithId(userId.Value)
                                            .FirstOrDefault();

                return infosUtil;
            }
        }

        private static List<IUtilisateur> GetAllUsersWithId(long id)
        {
            var clientsWithId = GetClientsWithId(id).ToList();
            var vendeursWithId = GetVendeursWithId(id).ToList();
            var gestionnaireWithId = GetAdminsWithId(id).ToList();
            
            return clientsWithId.Concat(vendeursWithId).Concat(gestionnaireWithId).ToList();
        }

        private static IQueryable<IUtilisateur> GetClientsWithId(long id)
        {
            return from util
                    in ctxt.PPClients
                where util.NoClient == id
                select util;
        }

        private static IQueryable<IUtilisateur> GetVendeursWithId(long id)
        {
            return from util
                    in ctxt.PPVendeurs
                where util.NoVendeur == id
                select util;
        }
        
        private static IQueryable<IUtilisateur> GetAdminsWithId(long id)
        {
            return from util
                    in ctxt.PPGestionnaires
                where util.NoGestionnaire == id
                select util;
        }
    }
}