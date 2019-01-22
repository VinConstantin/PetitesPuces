using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PetitesPuces.Models;

namespace PetitesPuces.Controllers
{
    public class PanierController : ApiController
    {
        public IHttpActionResult Post(int idProduit)
        {
            return Ok();
        }
    }
}