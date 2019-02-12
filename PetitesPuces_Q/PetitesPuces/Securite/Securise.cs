using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http.Controllers;
using System.Web.Routing;

namespace PetitesPuces.Securite
{
    public class Securise: AuthorizeAttribute
    {
        private string[] rolesAutorises;

        public Securise()
        {
            this.rolesAutorises = new string[] {"client", "vendeur", "admin"};
        }

        public Securise(params string[] rolesAutorises)
        {
            this.rolesAutorises = rolesAutorises;
        }

        protected override bool AuthorizeCore(HttpContextBase actionContext)
        {
            var user = SessionUtilisateur.UtilisateurCourant;

            if (user == null) return false;

            return rolesAutorises.Contains(user.Role);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"Controller", "Home"},
                    {"Action", "Connexion"}
                }
            );
        }
    }
}