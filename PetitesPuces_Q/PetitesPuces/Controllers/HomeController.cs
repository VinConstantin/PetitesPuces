using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetitesPuces.Models;

namespace TestMVC.Controllers
{
    public class HomeController : Controller
    {
        BDPetitesPucesDataContext context = new BDPetitesPucesDataContext();
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Client");
        }

        public ActionResult Connexion()
        {
            var query = from whatever in context.PPClients
                        select whatever;
            var clients = query.ToList();
            return View(clients);
        }
        public ActionResult OubliMDP()
        {
            return View();
        }
        public ActionResult Catalogue()
        {
            return RedirectToAction("Catalogue", "Client");


        }
    }
}