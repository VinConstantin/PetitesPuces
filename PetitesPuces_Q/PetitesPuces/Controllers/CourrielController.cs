
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using PetitesPuces.Models;
using PetitesPuces.ViewModels.Courriel;
using PetitesPuces.ViewModels.Home;

namespace PetitesPuces.Controllers
{
    public class CourrielController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();

        public ActionResult Index()
        {          
            return View();
        }
        public ActionResult BoiteReception()
        {          
            return PartialView("Courriel/_BoiteReception");
        }
        public ActionResult Brouillons()
        {          
            return PartialView("Courriel/_Brouillons");
        }
        public ActionResult ElementsEnvoyes()
        {          
            return PartialView("Courriel/_ElementsEnvoyes");
        }
        public ActionResult ElementsSupprimes()
        {          
            return PartialView("Courriel/_ElementsSupprimes");
        }
        public ActionResult ComposerMessage()
        {          
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