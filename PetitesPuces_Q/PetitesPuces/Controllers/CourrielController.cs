
using System;
using System.Collections.Generic;
using System.Data.Linq;
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
        private const int MAX_FILE_SIZE_BYTES = 50 * 1000;
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


            if (Enum.TryParse(etatCourriel, out EtatCourriel enumEtat))
            {
                var isWrite = CheckIfWriteState(enumEtat);

                var message = GetMessageById(id, isWrite);

                return View(Tuple.Create(enumEtat, message));
            }

            return HttpNotFound();
        }

        private bool CheckIfWriteState(EtatCourriel etat)
        {
            switch (etat)
            {
                case EtatCourriel.Brouillon:
                case EtatCourriel.Composer:
                case EtatCourriel.Envoye:
                    return true;
                case EtatCourriel.Reception:
                case EtatCourriel.Supprime:
                    return false;
                default:
                    return true;
            }
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
                    case EtatCourriel.Envoye: return ElementsEnvoyes(id);
                    case EtatCourriel.Reception: return BoiteReception(id);
                    case EtatCourriel.Composer: return ComposerMessage(id);
                    case EtatCourriel.Supprime: return ElementsSupprimes(id);
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
                double maxMB = MAX_FILE_SIZE_BYTES / 1000.0;
                throw new InvalidDataException("La grandeur du fichier doit être inférieure " + maxMB + "Mo");
            }

            return file;
        }

        private void SaveAttachmentToMessage(HttpPostedFileBase file, PPMessage msg)
        {
            Directory.CreateDirectory(Server.MapPath(ATTACHMENTS_DIR));
            file.SaveAs(GetAttachmentPathForMessage(msg));

            msg.FichierJoint = file.FileName;

            context.SubmitChanges();
        }

        private string GetAttachmentPathForMessage(PPMessage message)
        {
            return Server.MapPath(ATTACHMENTS_DIR + "/attachment-" + message.NoMsg);
        }

        [HttpGet]
        [Route("Courriel/PieceJointe/{id}")]
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

        [HttpDelete]
        [Route("Courriel/PieceJointe/{id}")]
        public ActionResult PieceJointeSupprimer(long id)
        {
            try
            {
                var message = GetMessageById(id, true);

                DeleteAttachmentOfMessage(message);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        private void DeleteAttachmentOfMessage(PPMessage message)
        {
            var physicalPath = GetAttachmentPathForMessage(message);
            var fileInf = new FileInfo(physicalPath);

            if (!fileInf.Exists) throw new IOException("Aucune piece jointe pour ce message");
            fileInf.Delete();
        }

        private PPMessage GetMessageById(long id, bool write = false)
        {
            var message =
                (from msg
                        in context.PPMessages
                    where msg.NoMsg == id
                    select msg).First();

            var noUtil = SessionUtilisateur.NoUtilisateur;
            if ((!write && message.PPDestinataires.Any(dest => dest.NoDestinataire == noUtil)) ||
                noUtil == message.NoExpediteur)
            {
                return message;
            }
            else
            {
                throw new AuthenticationException("NoMsg invalide");
            }
        }

        /** PPLieu
            1	Boîte de réception
            2	Boîte envoyé
            3	Boîte supprimés
            4	Brouillon
            5	Supprimé définitevement
         */
        public ActionResult BoiteReception(int? id = null)
        {
            if (id.HasValue)
            {
                return GetCourriel(id.Value);
            }
            else
            {
                return AfficherBoiteReception();
            }
        }

        private ActionResult AfficherBoiteReception()
        {
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending
                where m.PPDestinataires.Any(d => d.NoDestinataire == noUtilisateur
                                                 && d.Lieu == (short?) LieuxCourriel.Reception)
                select m).ToList();

            return PartialView("Courriel/Boites/_BoiteReception", messages);
        }

        public ActionResult Brouillons()
        {
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending
                where m.NoExpediteur == noUtilisateur
                      && m.Lieu == (short?) LieuxCourriel.Brouillon
                select m).ToList();

            return PartialView("Courriel/Boites/_Brouillons", messages);
        }

        public ActionResult ElementsEnvoyes(int? id)
        {
            if (id.HasValue)
            {
                return GetCourriel(id.Value);
            }
            else
            {
                return AfficherBoiteEnvoyes();
            }
        }

        private ActionResult AfficherBoiteEnvoyes()
        {
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending
                where m.NoExpediteur == noUtilisateur && m.Lieu == (short?) LieuxCourriel.Envoye
                select m).ToList();

            return PartialView("Courriel/Boites/_ElementsEnvoyes", messages);
        }

        public ActionResult ElementsSupprimes(int? id)
        {
            if (id.HasValue)
            {
                return GetCourriel(id.Value);
            }
            else
            {
                return AfficherBoiteSupprimes();
            }
        }

        private ActionResult AfficherBoiteSupprimes()
        {
            List<PPMessage> messages = (from m in context.PPMessages
                orderby m.dateEnvoi descending
                where m.PPDestinataires.Any(d => d.NoDestinataire == noUtilisateur
                                                 && d.Lieu == (short?) LieuxCourriel.Archive)
                select m).ToList();

            return PartialView("Courriel/Boites/_ElementsSupprimes", messages);
        }

        public ActionResult ComposerMessage(int? id = null)
        {
            if (id.HasValue)
            {
                return PartialView("Courriel/_ComposerMessage", GetMessageById(id.Value, true));
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
            switch (SessionUtilisateur.UtilisateurCourant.Role)
            {
                case RolesUtil.CLIENT:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireClient", viewModel);
                case RolesUtil.VEND:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireVendeur", viewModel);
                case RolesUtil.ADMIN:
                    return PartialView("Courriel/AjoutDestinataire/_ModalAjoutDestinataireGestionnaire", viewModel);
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
            if (message != null) message.EtatLu = (short?) EtatLu.Lu;

            try
            {
                context.SubmitChanges();
                return PartialView("Courriel/_Courriel", messages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost]
        public ActionResult ApercuCourriel(BrouillonViewModel brouillon)
        {
            brouillon.NoMsg = -1;
            PPMessage message = EnregistrerMessage(brouillon);

            try
            {
                PPVendeur vendeur =
                    (from v in context.PPVendeurs
                        where v.NoVendeur == brouillon.destinataires.FirstOrDefault()
                        select v).FirstOrDefault();
                if (vendeur != null)
                {
                    message.DescMsg = message.DescMsg.Replace("X%", (vendeur.Pourcentage * 100).ToString() + "%");
                }
            }
            catch (Exception)
            {

            }
            

            RejectAllChanges(); //Au cas ou

            return View(message);
        }

        private void RejectAllChanges()
        {
            var changeset = context.GetChangeSet();

            context.Refresh(RefreshMode.OverwriteCurrentValues, changeset.Deletes);
            context.Refresh(RefreshMode.OverwriteCurrentValues, changeset.Updates);

            //Undo Inserts
            foreach (object objToInsert in changeset.Inserts)
            {
                context.GetTable(objToInsert.GetType()).DeleteOnSubmit(objToInsert);
            }

            //Undo deletes
            foreach (object objToDelete in changeset.Deletes)
            {
                context.GetTable(objToDelete.GetType()).InsertOnSubmit(objToDelete);
            }
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
                if (message != null) message.EtatLu = (short) EtatLu.Lu;
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
                if (message != null) message.EtatLu = (short?) EtatLu.NonLu;
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
                if (message != null) message.Lieu = (short?) LieuxCourriel.Reception;
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
                PPMessage message = (from m in context.PPMessages
                    where m.NoMsg == no && m.NoExpediteur == noUtilisateur
                    select m).FirstOrDefault();

                //si c'est l'expediteur, supprimer définitivement
                if (message != null)
                {
                    message.Lieu = (short?)LieuxCourriel.Supprime;
                }

                PPDestinataire destinataire = (from m in context.PPDestinataires
                    where m.NoMsg == no && m.NoDestinataire == noUtilisateur
                    select m).FirstOrDefault();
                
                if (destinataire != null)
                {
                    //si déja dans les elements supprimés, supprimer définitivement
                    if (destinataire.Lieu == (short?)LieuxCourriel.Archive)
                    {
                        destinataire.Lieu = (short?) LieuxCourriel.Supprime;
                    }
                    else
                    {
                        destinataire.Lieu = (short?) LieuxCourriel.Archive;
                    }
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
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Envoyer(int id)
        {
            var msg = GetMessageById(id, true);

            if (!msg.PPDestinataires.Any())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,
                    "Un message ne peux pas être envoyé sans destinataires");
            }
            else if (msg.PPLieu != null && msg.PPLieu.NoLieu != (short) LieuxCourriel.Brouillon)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Ce message semble déjà avoir été envoyé");
            }

            msg.PPLieu = (from l in context.PPLieus where l.NoLieu == (short) LieuxCourriel.Envoye select l).First();

            foreach (var dest in msg.PPDestinataires)
            {
                dest.EtatLu = (short) EtatLu.NonLu;
                dest.Lieu = (short) LieuxCourriel.Reception;
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

            return PartialView("Courriel/_MessageEnvoye");
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

            brouillon.destinataires = brouillon.destinataires ?? new List<int?>();

            msg.NoMsg = (int) brouillon.NoMsg.GetValueOrDefault(msg.NoMsg);
            msg.DescMsg = brouillon.DescMsg;
            msg.Lieu = brouillon.Lieu;
            msg.objet = brouillon.objet;
            msg.dateEnvoi = DateTime.Now;
            msg.NoExpediteur = (int) SessionUtilisateur.UtilisateurCourant.No;

            var destinataires = msg.PPDestinataires.ToList();
            var NosDestinatairesAAjouter = new List<int?>(brouillon.destinataires);
            var NosDestinatairesADelete = new List<int>();
            foreach (var dest in destinataires)
            {
                if (dest.NoDestinataire < 0 && dest.NoDestinataire > -4)
                {
                    context.PPDestinataires.DeleteAllOnSubmit(msg.PPDestinataires);
                    msg.PPDestinataires.AddRange(ConvToDestinataire(CasSpeciaux(dest.NoDestinataire), msg));
                    return msg;
                }

                if (!brouillon.destinataires.Contains(dest.NoDestinataire))
                {
                    NosDestinatairesADelete.Add(dest.NoDestinataire);
                }
                else
                {
                    NosDestinatairesAAjouter.Remove(dest.NoDestinataire);
                }
            }

            var destinatairesADelete = (from dest in context.PPDestinataires
                where NosDestinatairesADelete.Contains(dest.NoDestinataire)
                select dest);
            context.PPDestinataires.DeleteAllOnSubmit(destinatairesADelete);

            List<PPDestinataire> destinatairesObj = new List<PPDestinataire>(NosDestinatairesAAjouter.Count);
            foreach (var noDest in NosDestinatairesAAjouter)
            {
                if (noDest < 0)
                {
                    context.PPDestinataires.DeleteAllOnSubmit(msg.PPDestinataires);
                    msg.PPDestinataires.AddRange(ConvToDestinataire(CasSpeciaux((int)noDest), msg));
                    return msg;
                }

                if (!noDest.HasValue) continue;

                var objDest = new PPDestinataire
                {
                    NoMsg = msg.NoMsg,
                    NoDestinataire = noDest.Value
                };
                destinatairesObj.Add(objDest);
            }

            msg.PPDestinataires.AddRange(destinatairesObj);
            context.PPDestinataires.InsertAllOnSubmit(destinatairesObj);

            return msg;
        }

        private List<PPDestinataire> ConvToDestinataire(List<IUtilisateur> utils, PPMessage message)
        {
            return utils.Select(u => new PPDestinataire
            {
                EtatLu = (short) EtatLu.NonLu, Lieu = (short) LieuxCourriel.Reception, NoDestinataire = (int)u.No
            }).ToList();
        }

        private List<IUtilisateur> CasSpeciaux(long id)
        {
            if (id == (int)CasSpeciauxDestinataire.Tous)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return context.PPClients.ToList().Concat<IUtilisateur>(context.PPVendeurs.ToList()).ToList().Concat(context.PPGestionnaires.Where(g => g.NoGestionnaire != SessionUtilisateur.NoUtilisateur).ToList()).ToList();
                    case RolesUtil.VEND:
                        return context.PPClients.Where(c => c.PPCommandes.Any(comm => comm.NoVendeur == SessionUtilisateur.NoUtilisateur)).ToList().Concat<IUtilisateur>(context.PPGestionnaires.ToList()).ToList();
                    case RolesUtil.CLIENT:
                        return context.PPVendeurs.ToList().Concat<IUtilisateur>(context.PPGestionnaires.ToList()).ToList();
                }
            }
            else if (id == (int)CasSpeciauxDestinataire.TousClients)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return context.PPClients.Cast<IUtilisateur>().ToList();
                    case RolesUtil.VEND:
                        return context.PPClients.Where(c => c.PPCommandes.Any(comm => comm.NoVendeur == SessionUtilisateur.NoUtilisateur)).Cast<IUtilisateur>().ToList();
                    default:
                        throw new AuthenticationException("Un client ne peux pas envoyer un message à tous les clients");
                }
            }
            else if (id == (int)CasSpeciauxDestinataire.TousVendeurs)
            {
                switch (SessionUtilisateur.UtilisateurCourant.Role)
                {
                    case RolesUtil.ADMIN:
                        return context.PPClients.Cast<IUtilisateur>().ToList();
                    case RolesUtil.CLIENT:
                        return context.PPVendeurs.Cast<IUtilisateur>().ToList();
                    default:
                        throw new AuthenticationException("Un vendeur ne peux pas envoyer un message à tous les vendeurs");
                }
            }

            throw new Exception();
        }

        private enum CasSpeciauxDestinataire
        {
            Tous = -1,
            TousVendeurs = -2,
            TousClients = -3,
        }

        [HttpPost]
        public ActionResult EnregistrerTransfer(int noCourriel)
        {
            PPMessage ogMessage = GetMessageById(noCourriel);

            var maxId = (from m in context.PPMessages
                select m.NoMsg).ToList().DefaultIfEmpty().Max();
            int noMessage = maxId + 1;
            var obj = "tr: " + ogMessage.objet;
            var enteteTransfert = "<br/><hr/>" +
                                  "<p>---------- message transféré ---------</p>" +
                                  "<p>De : " + ogMessage.Expediteur.DisplayName + "</p>" +
                                  "<p>Date : " + ogMessage.dateEnvoi.Value.ToLongDateString() + "</p>" +
                                  "<p>Objet : " + ogMessage.objet + "</p>" +
                                  "<br/>";
            var description = enteteTransfert + ogMessage.DescMsg;

            PPMessage brouillon = new PPMessage
            {
                objet = obj,
                DescMsg = description,
                dateEnvoi = DateTime.Now,
                Lieu = 4,
                NoMsg = noMessage,
                FichierJoint = ogMessage.FichierJoint,
                NoExpediteur = (int) SessionUtilisateur.UtilisateurCourant.No
            };
            context.PPMessages.InsertOnSubmit(brouillon);

            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return Content(noMessage.ToString());
        }

        [HttpPost]
        public ActionResult EnregistrerReponse(int noCourriel)
        {
            PPMessage ogMessage = GetMessageById(noCourriel);

            var maxId = (from m in context.PPMessages
                select m.NoMsg).ToList().DefaultIfEmpty().Max();
            int noMessage = maxId + 1;
            var obj = "re: " + ogMessage.objet;
            var enteteReponse = "<br/><hr/>" +
                                "<p>---------- message repondu ---------</p>" +
                                "<p>De : " + ogMessage.Expediteur.DisplayName + "</p>" +
                                "<p>Date : " + ogMessage.dateEnvoi.Value.ToLongDateString() + "</p>" +
                                "<p>Objet : " + ogMessage.objet + "</p>" +
                                "<br/>";
            var description = enteteReponse + ogMessage.DescMsg;

            PPMessage brouillon = new PPMessage
            {
                objet = obj,
                DescMsg = description,
                dateEnvoi = DateTime.Now,
                Lieu = 4,
                NoMsg = noMessage,
                FichierJoint = ogMessage.FichierJoint,
                NoExpediteur = (int) SessionUtilisateur.UtilisateurCourant.No
            };
            context.PPMessages.InsertOnSubmit(brouillon);
            context.PPDestinataires.InsertOnSubmit(
                new PPDestinataire
                {
                    NoMsg = noMessage,
                    NoDestinataire = (int) ogMessage.NoExpediteur
                }
            );
            try
            {
                context.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return Content(noMessage.ToString());
        }
    }

    public enum EtatLu : short
    {
        NonLu = 0,
        Lu = 1,
    }

    public enum LieuxCourriel : short
    {
        Reception = 1,
        Envoye = 2,
        Archive = 3,
        Brouillon = 4,
        Supprime = 5,
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