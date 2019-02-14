
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Web.Mvc;
using System.Web;
using PetitesPuces.Models;
using PetitesPuces.Models.Courriel;
using PetitesPuces.Securite;
using PetitesPuces.ViewModels.Courriel;

namespace PetitesPuces.Controllers
{
    #if !DEBUG
        [Securise]
    #endif
    public class CourrielController : Controller
    {
        private readonly long noUtilisateur = SessionUtilisateur.UtilisateurCourant.No;
        private readonly BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        private const int MAX_FILE_SIZE_BYTES = 50  * 1000;
        private const string ATTACHMENTS_DIR = "/Envoyés";
        
        [Route("Courriel")]
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return BoiteReception();
            }
            else
            {
                return View(Tuple.Create<EtatCourriel, PPMessage>(EtatCourriel.Reception, null));
            }
        }

        [Route("Courriel/Boite/{etatCourriel}/{id}")]
        public ActionResult Index(string etatCourriel, int id)
        {
            if (Request.IsAjaxRequest()) return IndexAjax(etatCourriel, id);

            var message = GetMessageById(id);

            if (Enum.TryParse(etatCourriel, out EtatCourriel enumEtat))
            {
                return View(Tuple.Create(enumEtat, message));
            }

            return HttpNotFound();
        }

        [Route("Courriel/Boite/{etatCourriel}")]
        public ActionResult Index(string etatCourriel)
        {
            if (Request.IsAjaxRequest()) return IndexAjax(etatCourriel);
            
            if (Enum.TryParse(etatCourriel, out EtatCourriel enumEtat)) 
            {
                return View(Tuple.Create<EtatCourriel, PPMessage>(enumEtat, null));
            }
            
            return HttpNotFound();
        }

        private ActionResult IndexAjax(string etatCourriel, int? id = null)
        {
            if (Enum.TryParse(etatCourriel, out EtatCourriel enumEtat)) 
            {
                switch (enumEtat)
                {
                    case EtatCourriel.Envoye   : return ElementsEnvoyes();
                    case EtatCourriel.Reception: return BoiteReception();
                    case EtatCourriel.Composer : return ComposerMessage(id);
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
            catch (Exception e) when (e is InvalidOperationException || e is AuthenticationException)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "NoMsg invalide");
            }
        }

        private HttpPostedFileBase GetFileFromRequest()
        {
            if (Request.Files.Count == 0)
            {
                throw new InvalidDataException("Aucun fichier téléversé");
            }

            var file = Request.Files[0];

            if (file == null)
            {
                throw new InvalidDataException("Aucun fichier téléversé");
            }

            if (file.ContentLength > MAX_FILE_SIZE_BYTES)
            {
                throw new InvalidDataException("La grandeur du fichier doit être inférieure 50Mo");
            }

            return file;
        }

        private void SaveAttachmentToMessage(HttpPostedFileBase file, PPMessage msg)
        {
            Directory.CreateDirectory(Server.MapPath(ATTACHMENTS_DIR));
            file.SaveAs(Server.MapPath(ATTACHMENTS_DIR + "/attachment-" + msg.NoMsg));

            msg.FichierJoint = file.FileName;

            context.SubmitChanges();
        }

        [HttpGet]
        public ActionResult PieceJointe(long id)
        {
            try
            {
                var message = GetMessageById(id);
                var fileType = message.FichierJoint.ToString().Split('.').Last();

                var fs = new FileStream(Server.MapPath(ATTACHMENTS_DIR + "/attachment-" + message.NoMsg), FileMode.Open,
                    FileAccess.Read);
                var mimeType = MimeTypeMap.GetMimeType(fileType);

                return File(fs, mimeType, message.FichierJoint.ToString());
            }
            catch (InvalidOperationException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "NoMsg invalide");
            }
            catch (IOException e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Fichier manquant ou suuprimé");
            }
        }

        private PPMessage GetMessageById(long id)
        {
            var message =
                (from msg
                    in context.PPMessages
                where msg.NoMsg == id
                select msg).First();

            if (SessionUtilisateur.UtilisateurCourant.No != message.NoExpediteur)
            {
                throw new AuthenticationException();
            }

            return message;
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
                where m.NoExpediteur == noUtilisateur
                      && m.Lieu == 4
                select m).ToList();


            return PartialView("Courriel/Boites/_Brouillons", messages);
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
        public ActionResult ComposerMessage(int? id = null)
        {      
            List<PPMessage> messages = (from m in context.PPMessages
                where m.PPDestinataires.Any(d=>d.NoDestinataire == noUtilisateur
                                               && d.Lieu == 1)
                select m).ToList();

            if (id.HasValue)
            {
                return PartialView("Courriel/_ComposerMessage", GetMessageById(id.Value));
            }
            else
            {
                return PartialView("Courriel/_ComposerMessage");
            }
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
        [Route("Courriel/Enregistrer")]
        public ActionResult CreerBrouillon(BrouillonViewModel brouillon)
        {
            var maxId = (from m in context.PPMessages
                select m.NoMsg).ToList().DefaultIfEmpty().Max();

            int noMessage = maxId + 1;
            brouillon.NoMsg = noMessage;
            var message = EnregistrerMessage(brouillon);

            context.PPMessages.InsertOnSubmit(message);
            context.SubmitChanges();

            return Json(brouillon);
        }
        
        [HttpPut]
        [Route("Courriel/Enregistrer")]
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

            brouillon.destinataires = brouillon.destinataires ?? new List<int>();

            msg.NoMsg = (int) brouillon.NoMsg.GetValueOrDefault(msg.NoMsg);
            msg.DescMsg = brouillon.DescMsg;
            msg.Lieu = brouillon.Lieu;
            msg.objet = brouillon.objet;
            msg.dateEnvoi = DateTime.Now;
            msg.NoExpediteur = (int)SessionUtilisateur.UtilisateurCourant.No;

            var destinataires = msg.PPDestinataires.ToList();
            List<int> NosDestinatairesAAjouter = new List<int>(brouillon.destinataires);
            foreach (var dest in destinataires)
            {
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