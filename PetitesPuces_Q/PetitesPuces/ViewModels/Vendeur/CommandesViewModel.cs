using PetitesPuces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetitesPuces.ViewModels.Vendeur
{
    public class CommandesViewModel
    {
        public List<Commande> CommandesATraiter { get; set; }
        public List<Commande> CommandesHistorique { get; set; }
    }
}