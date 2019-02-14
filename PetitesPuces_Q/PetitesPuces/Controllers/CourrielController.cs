
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web;
using System.Web.Http.Results;
using PetitesPuces.Models;
using PetitesPuces.Models.Courriel;
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
        private readonly long noUtilisateur = SessionUtilisateur.UtilisateurCourant.No;
        private readonly BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        private const string ATTACHMENTS_FOLDER = "~/Attachments/";

        [Route("Courriel")]
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return BoiteReception();
            }
            else
            {
                return View(EtatCourriel.Reception);
            }
        }
        
        [Route("Courriel/Boite/{etatCourriel}")]
        public ActionResult Index(string etatCourriel)
        {
            if (Request.IsAjaxRequest()) return IndexAjax(etatCourriel);

            EtatCourriel enumEtat;
            if (Enum.TryParse(etatCourriel, out enumEtat)) 
            {
                return View(enumEtat);
            }
            
            return HttpNotFound();
        }

        private ActionResult IndexAjax(string etatCourriel)
        {
            EtatCourriel enumEtat;
            if (Enum.TryParse(etatCourriel, out enumEtat)) 
            {
                switch (enumEtat)
                {
                    case EtatCourriel.Envoye   : return ElementsEnvoyes();
                    case EtatCourriel.Reception: return BoiteReception();
                    case EtatCourriel.Composer : return ComposerMessage();
                    case EtatCourriel.Supprime : return ElementsSupprimes();
                    case EtatCourriel.Brouillon: return Brouillons();
                }
            }
            
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Televerser(long id)
        {
            try
            {
                var attachment = GetFileFromRequest();
                var message = GetMessageById(id);
                SaveAttachmentToMessage(attachment, message);

                return new HttpStatusCodeResult(200);
            }
            catch (InvalidDataException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, e.Message);
            }
            catch (InvalidOperationException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid message ID");
            }
        }
        
        [HttpGet]
        public ActionResult PieceJointe(long id)
        {
            try
            {
                var message = GetMessageById(id);

                FileStream fs = new FileStream(Server.MapPath(ATTACHMENTS_FOLDER+"/attachment-"+message.NoMsg), FileMode.Open, FileAccess.Read);
                
                return File(fs, "application/octet-stream", (string)message.FichierJoint);
            }
            catch (InvalidOperationException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid message ID");
            }
        }
        
        private HttpPostedFileBase GetFileFromRequest()
        {
            if (Request.Files.Count == 0)
            {
                throw new InvalidDataException("No file posted");
            }

            var file = Request.Files[0];

            if (file == null)
            {
                throw new InvalidDataException( "No file posted");
            }

            return file;
        }

        private PPMessage GetMessageById(long id)
        {
            return
                (from msg
                    in context.PPMessages
                where msg.NoMsg == id
                select msg).First();
        }

        private void SaveAttachmentToMessage(HttpPostedFileBase file, PPMessage msg)
        {
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);

            byte[] data = target.ToArray();
            msg.FichierJoint = new Binary(data);

            context.SubmitChanges();
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
                orderby m.dateEnvoi descending 
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 1)             
                select m).ToList();
            
            return PartialView("Courriel/Boites/_BoiteReception",messages);
        }
        public ActionResult Brouillons()
        {        
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending 
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 4)
                select m).ToList();
            
            return PartialView("Courriel/Boites/_Brouillons",messages);
        }
        public ActionResult ElementsEnvoyes()
        {         
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending 
                where m.NoExpediteur == noUtilisateur && m.Lieu == 2
                select m).ToList();
            
            return PartialView("Courriel/Boites/_ElementsEnvoyes",messages);
        }
        public ActionResult ElementsSupprimes()
        {          
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending 
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 3)
                select m).ToList();
            
            return PartialView("Courriel/Boites/_ElementsSupprimes",messages);
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
            switch (SessionUtilisateur.UtilisateurCourant.Role)
            {
                case RolesUtil.CLIENT:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireClient",viewModel);
                case RolesUtil.VEND:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireVendeur",viewModel);
                case RolesUtil.ADMIN:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireGestionnaire",viewModel);
                default:
                    return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Role introuvable");
            }
        }
        
        /// <summary>
        /// Permet d'aller checker un courriel en particulier
        /// </summary>
        /// <param name="NoCourriel"></param>
        /// <returns>le partialview du courriel</returns>
        public ActionResult GetCourriel(int NoCourriel = 0)
        {
            PPMessage messages = (from m in context.PPMessages
                where m.NoMsg == NoCourriel
                select m).FirstOrDefault();
            
            PPDestinataire message = (from m in context.PPDestinataires
                where m.NoMsg == NoCourriel && m.NoDestinataire == noUtilisateur
                select m).FirstOrDefault();
            if (message != null) message.EtatLu = 1;
            
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return PartialView("Courriel/_Courriel", messages);
        }
        
        /// <summary>
        /// Marque les courriels lu
        /// </summary>
        /// <param name="NoCourriel"></param>
        /// <returns>si ok</returns>
        [HttpPost]
        public ActionResult MarquerLu(List<int> NoCourriel)
        {
            foreach (int no in NoCourriel)
            {
                PPDestinataire message = (from m in context.PPDestinataires
                    where m.NoMsg == no && m.NoDestinataire == noUtilisateur
                    select m).FirstOrDefault();
                if (message != null) message.EtatLu = 1;
            }
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        
        /// <summary>
        /// Marque les courriels non-lu
        /// </summary>
        /// <param name="NoCourriel"></param>
        /// <returns>si ok</returns>
        [HttpPost]
        public ActionResult MarquerNonLu(List<int> NoCourriel)
        {
            foreach (int no in NoCourriel)
            {
                PPDestinataire message = (from m in context.PPDestinataires
                    where m.NoMsg == no && m.NoDestinataire == noUtilisateur
                    select m).FirstOrDefault();
                if (message != null) message.EtatLu = 0;
            }
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        
        /// <summary>
        /// deplace les couriels des elements suprimés à la boite de reception
        /// </summary>
        /// <param name="NoCourriel"></param>
        /// <returns>si ok</returns>
        [HttpPost]
        public ActionResult Restaurer(List<int> NoCourriel)
        {
            foreach (int no in NoCourriel)
            {
                PPDestinataire message = (from m in context.PPDestinataires
                    where m.NoMsg == no && m.NoDestinataire == noUtilisateur
                    select m).FirstOrDefault();
                if (message != null) message.Lieu = 1;
            }
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        
        /// <summary>
        /// deplace les couriels vers les elements supprimes ou si déja supprimé, supprime definitivemenet
        /// </summary>
        /// <param name="NoCourriel"></param>
        /// <returns>si ok</returns>
        [HttpDelete]
        public ActionResult Supprimer(List<int> NoCourriel)
        {
            foreach (int no in NoCourriel)
            {
                PPDestinataire message = (from m in context.PPDestinataires
                    where m.NoMsg == no && m.NoDestinataire == noUtilisateur
                    select m).FirstOrDefault();
                if (message != null)
                {
                    //si déja dans les elements supprimés, supprimer définitivement
                    if (message.Lieu == 3)
                    {
                        message.Lieu = 5;
                    }
                    else
                    {
                        message.Lieu = 3;
                    }
                    //For some reason that doesn't work ... : message.Lieu = (message.Lieu == 3) ? (short)5 : (short)3;
                }
            }
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        
        /// <summary>
        /// Permet de creer le message et le destinataire et de les inserer dans la BD
        /// </summary>
        /// <param name="NoExpediteur"></param>
        /// <param name="Objet"></param>
        /// <param name="Description"></param>
        /// <param name="Destinataire"></param>
        /// <returns>si ok</returns>
        [HttpPost]
        [ValidateInput(false)] 
        public ActionResult Envoyer(int NoExpediteur, string Objet, string Description, List<Destinataire> Destinataire)
        {
            var queryNo = from m in context.PPMessages
                orderby m.NoMsg descending
                select m;
            
            int noMessage = queryNo.Any()?queryNo.FirstOrDefault().NoMsg + 1: 1;
            
            PPMessage message = new PPMessage
            {
                NoExpediteur = NoExpediteur,
                objet = Objet,
                DescMsg = Description,
                dateEnvoi = DateTime.Now,
                Lieu = 2, //éléments envoyés
                NoMsg = noMessage,
                FichierJoint = null //TODO: implémenter le telechargement de fichier
                
            };
            context.PPMessages.InsertOnSubmit(message);
            foreach (Destinataire dest in Destinataire)
            {
                context.PPDestinataires.InsertOnSubmit(
                    new PPDestinataire
                    {
                        NoMsg = noMessage,
                        Lieu = 1, //Boîte de réception
                        NoDestinataire = (int) dest.No,
                        EtatLu = 0
                    }
                );
            }
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Enregistrer")]
        public ActionResult CreerBrouillon(BrouillonViewModel brouillon)
        {
            brouillon.NoMsg = (from msg in context.PPMessages select msg.NoMsg).Max() + 1;
            var message = EnregistrerMessage(brouillon);

            context.PPMessages.InsertOnSubmit(message);
            context.SubmitChanges();

            return Json(brouillon);
        }
        
        [HttpPut]
        [Route("Enregistrer")]
        public ActionResult EnregistrerBrouillon(BrouillonViewModel brouillon)
        {
            var ogMessage = (from msg in context.PPMessages where msg.NoMsg == brouillon.NoMsg select msg).First();
            var message = EnregistrerMessage(brouillon, ogMessage);

            context.SubmitChanges();

            return Json(brouillon);
        }

        private PPMessage EnregistrerMessage(BrouillonViewModel brouillon, PPMessage msg = null)
        {
            if (msg == null)
            {
                msg = new PPMessage();
            }

            msg.DescMsg = brouillon.DescMsg;
            msg.Lieu = brouillon.Lieu;
            msg.objet = brouillon.objet;
            msg.NoExpediteur = (int)SessionUtilisateur.UtilisateurCourant.No;

            var destinataires = msg.PPDestinataires.ToList();
            List<int> NosDestinatairesAAjouter = new List<int>(brouillon.destinataires);
            for (int i = 0; i < destinataires.Count; i++)
            {
                var dest = destinataires[i];

                if (!brouillon.destinataires.Contains(dest.NoDestinataire))
                {
                    msg.PPDestinataires.Remove(dest);
                }
                else
                {
                    NosDestinatairesAAjouter.Remove(dest.NoDestinataire);
                }
            }

            foreach (var NoDest in NosDestinatairesAAjouter)
            {
                context.PPDestinataires.InsertOnSubmit(
                    new PPDestinataire
                    {
                        NoMsg = msg.NoMsg,
                        NoDestinataire = NoDest
                    }
                );
            }

            return msg;
        }
    }
    
    public enum EtatCourriel
    {
        Composer,
        Reception,
        Brouillon,
        Envoye,
        Supprime,
    }
}