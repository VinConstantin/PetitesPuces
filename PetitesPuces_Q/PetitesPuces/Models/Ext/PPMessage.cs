using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

                    _utilisateursDestinataire.Add(usersWithId.First());
                }

                return _utilisateursDestinataire;
            }
        }

        public IUtilisateur Expediteur
        {
            get
            {
                if (ctxt == null)
                {
                    ctxt = new BDPetitesPucesDataContext();
                }
                
                return GetAllUsersWithId((int)NoExpediteur).FirstOrDefault();
            }
        }
        private List<IUtilisateur> GetAllUsersWithId(int id)
        {
            var clientsWithId = GetClientsWithId(id).ToList();
            var vendeursWithId = GetVendeursWithId(id).ToList();
            var gestionnaireWithId = GetAdminsWithId(id).ToList();

            return clientsWithId.Concat(vendeursWithId).Concat(gestionnaireWithId).ToList();
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