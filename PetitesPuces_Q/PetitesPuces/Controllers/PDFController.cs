using IronPdf;
using PetitesPuces.Models;
using PetitesPuces.Securite;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;

using ExpertPdf.HtmlToPdf;

namespace PetitesPuces.Controllers
{
    public class PDFController : Controller
    {

        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        // GET: PDF
        public ActionResult Index()
        {
            return View();
        }

        [Securise(RolesUtil.CLIENT, RolesUtil.VEND)]
        public ActionResult InfoCommande(int id)
        {
            var user = SessionUtilisateur.UtilisateurCourant;

            var query = from commandes in context.PPCommandes
                        where commandes.NoCommande == id
                        select commandes;

            var commande = query.FirstOrDefault();

            if (user is PPVendeur)
            {
                PPVendeur vendeur = (PPVendeur)user;
                if (commande.PPVendeur.NoVendeur != vendeur.NoVendeur)
                    return new HttpStatusCodeResult(400, "Id commande invalide");
            }
            else if (user is PPClient)
            {
                PPClient client = (PPClient)user;
                if (commande.PPClient.NoClient != client.NoClient)
                    return new HttpStatusCodeResult(400, "Id commande invalide");
            }

            string path = Server.MapPath("/Recus/" + id + ".pdf");
           

            if (!System.IO.File.Exists(path))
                GenererPDF((int) commande.NoCommande);

            return File(path, "application/pdf");
        }

        public ActionResult GenererPDF(int id)
        {
            var commande = (from commandes in context.PPCommandes
                            where commandes.NoCommande == id
                            select commandes).SingleOrDefault();

            var user = SessionUtilisateur.UtilisateurCourant;

            if (user is PPVendeur)
            {
                PPVendeur vendeur = (PPVendeur)user;
                if (commande.PPVendeur.NoVendeur != vendeur.NoVendeur)
                    throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            }
            else if (user is PPClient)
            {
                PPClient client = (PPClient)user;
                if (commande.PPClient.NoClient != client.NoClient)
                    throw new HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
            }

            if (!Directory.Exists(Server.MapPath("/Recus")))
            {
                Directory.CreateDirectory(Server.MapPath("/Recus"));
            }

            string path = Server.MapPath("/Recus/" + commande.NoCommande + ".pdf");
            
            if (System.IO.File.Exists(path))
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            
            string view = PartialView("Vendeur/_RecuImpression", commande).RenderToString();

            PdfConverter pdfC = new PdfConverter();
            
            pdfC.SavePdfFromHtmlStringToFile(view, path);

            /*HtmlToPdf Renderer = new HtmlToPdf();
            var PDF = Renderer.RenderHtmlAsPdf(view);
            PDF.TrySaveAs(path);*/
            
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}