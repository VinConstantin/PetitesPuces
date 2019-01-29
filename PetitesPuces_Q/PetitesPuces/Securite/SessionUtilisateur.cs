using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetitesPuces.Models;

namespace PetitesPuces.Securite
{
    public static class SessionUtilisateur
    {
        private static IUtilisateur _utilisateurCourant;
        private static readonly BDPetitesPucesDataContext ctxt = new BDPetitesPucesDataContext();

        public static IUtilisateur UtilisateurCourant
        {
            get
            {
                if (_utilisateurCourant != null) return _utilisateurCourant;

                var userId = (long)HttpContext.Current.Session["userId"];

                IUtilisateur infosUtil = GetAllUsersWithId(userId)
                                            .FirstOrDefault();

                _utilisateurCourant = infosUtil;

                return infosUtil;
            }
        }

        private static IQueryable<IUtilisateur> GetAllUsersWithId(long id)
        {
            var clientsWithId = GetClientsWithId(id);
            var vendeursWithId = GetVendeursWithId(id);

            return clientsWithId.Concat(vendeursWithId);
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
    }
}