using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using PetitesPuces.Securite;

namespace PetitesPuces.Models
{
    public partial class PPMessage
    {
        private List<IUtilisateur> _utilisateursDestinataire;
        private BDPetitesPucesDataContext ctxt;

        public List<IUtilisateur> UtilisateursDestinataire
        {
            get
            {
                if (_utilisateursDestinataire != null) return _utilisateursDestinataire;

                if (ctxt == null)
                {
                    ctxt = new BDPetitesPucesDataContext();
                }

                _utilisateursDestinataire = new List<IUtilisateur>();

                foreach (var dest in PPDestinataires)
                {
                    var usersWithId = GetAllUsersWithId(dest.NoDestinataire);

                    if (dest.NoDestinataire == 0) continue;

                    _utilisateursDestinataire.Add(usersWithId.First());
                }

                return _utilisateursDestinataire;
            }
        }

        private List<IUtilisateur> GetAllUsersWithId(int id)
        {
            if (id == (int)CasSpeciauxDestinataire.Tous)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return ctxt.PPClients.ToList().Concat<IUtilisateur>(ctxt.PPVendeurs.ToList()).ToList().Concat(ctxt.PPGestionnaires.Where(g => g.NoGestionnaire != SessionUtilisateur.NoUtilisateur).ToList()).ToList();
                    case RolesUtil.VEND:
                        return ctxt.PPClients.Where(c => c.PPCommandes.Any(comm => comm.NoVendeur == SessionUtilisateur.NoUtilisateur)).ToList().Concat<IUtilisateur>(ctxt.PPGestionnaires.ToList()).ToList();
                    case RolesUtil.CLIENT:
                        return ctxt.PPVendeurs.ToList().Concat<IUtilisateur>(ctxt.PPGestionnaires.ToList()).ToList();
                }
            }
            else if (id == (int)CasSpeciauxDestinataire.TousClients)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return ctxt.PPClients.Cast<IUtilisateur>().ToList();
                    case RolesUtil.VEND:
                        return ctxt.PPClients.Where(c => c.PPCommandes.Any(comm => comm.NoVendeur == SessionUtilisateur.NoUtilisateur)).Cast<IUtilisateur>().ToList();
                    default:
                        throw new AuthenticationException("Un client ne peux pas envoyer un message à tous les clients");
                }
            } 
            else if (id == (int)CasSpeciauxDestinataire.TousVendeurs)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return ctxt.PPClients.Cast<IUtilisateur>().ToList();
                    case RolesUtil.CLIENT:
                        return ctxt.PPVendeurs.Cast<IUtilisateur>().ToList();
                    default:
                        throw new AuthenticationException("Un vendeur ne peux pas envoyer un message à tous les vendeurs");
                }
            }

            var clientsWithId = GetClientsWithId(id).ToList();
            var vendeursWithId = GetVendeursWithId(id).ToList();
            var gestionnaireWithId = GetAdminsWithId(id).ToList();

            return clientsWithId.Concat(vendeursWithId).Concat(gestionnaireWithId).ToList();
        }

        private enum CasSpeciauxDestinataire
        {
            Tous = -1,
            TousVendeurs = -2,
            TousClients = -3,
        }

        private IQueryable<IUtilisateur> GetClientsWithId(int id)
        {
            return from util
                    in ctxt.PPClients
                where util.NoClient == id
                select util;
        }

        private IQueryable<IUtilisateur> GetVendeursWithId(int id)
        {
            return from util
                    in ctxt.PPVendeurs
                where util.NoVendeur == id
                select util;
        }

        private IQueryable<IUtilisateur> GetAdminsWithId(int id)
        {
            return from util
                    in ctxt.PPGestionnaires
                where util.NoGestionnaire == id
                select util;
        }
    }
}