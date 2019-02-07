
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels.Courriel;
using PetitesPuces.ViewModels.Home;

namespace PetitesPuces.Controllers
{
    #if !DEBUG
        [Securise]
    #endif
    public class CourrielController : Controller
    {
        //private long noUtilisateur = SessionUtilisateur.UtilisateurCourant.No;
        private long noUtilisateur = 10;
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {                    
            return View();
        }

        public ActionResult GetCourriel(int NoCourriel = 0)
        {
            PPMessage messages = (from m in context.PPMessages
                where m.NoMsg == NoCourriel
                select m).FirstOrDefault();

            return PartialView("Courriel/_Courriel", messages);
        }

        [HttpDelete]
        public ActionResult Supprimer(int NoCourriel = 0)
        {
            PPDestinataire message = (from m in context.PPDestinataires
                where m.NoMsg == NoCourriel && m.NoDestinataire == noUtilisateur
            select m).FirstOrDefault();

            if (message != null) message.Lieu = 3;
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        /** PPLieu
            1	Boîte de réception
            2	Boîte envoyé
            3	Boîte supprimés
            4	Brouillon
            5	Supprimé définitevement
         */
        public ActionResult BoiteReception()
        {           
            List<PPMessage> messages = (from m in context.PPMessages
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 1)
                select m).ToList();
            
            return PartialView("Courriel/_BoiteReception",messages);
        }
        public ActionResult Brouillons()
        {        
            List<PPMessage> messages = (from m in context.PPMessages
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 4)
                select m).ToList();
            
            return PartialView("Courriel/_Brouillons",messages);
        }
        public ActionResult ElementsEnvoyes()
        {         
            List<PPMessage> messages = (from m in context.PPMessages
                where m.NoExpediteur == noUtilisateur && m.Lieu == 2
                select m).ToList();
            
            return PartialView("Courriel/_ElementsEnvoyes",messages);
        }
        public ActionResult ElementsSupprimes()
        {          
            List<PPMessage> messages = (from m in context.PPMessages
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 3)
                select m).ToList();
            
            return PartialView("Courriel/_ElementsSupprimes",messages);
        }
        public ActionResult ComposerMessage()
        {      
            List<PPMessage> messages = (from m in context.PPMessages
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 1)
                select m).ToList();
            
            return PartialView("Courriel/_ComposerMessage");
        }
        public ActionResult Destinataire()
        {
            DestinataireViewModel viewModel = new DestinataireViewModel
            {
                Clients = (from c in context.PPClients select c).ToList(),
                Vendeurs = (from v in context.PPVendeurs select v).ToList()
            };
            return PartialView("Courriel/_ModalAjoutDestinataire",viewModel);
        }
    }
}