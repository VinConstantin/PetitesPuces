using System.Collections.Generic;
using PetitesPuces.Controllers;

namespace PetitesPuces.ViewModels.Courriel
{
    public class BrouillonViewModel
    {
        public long? NoMsg { get; set; }
        public string DescMsg { get; set; }
        public short? Lieu { get; set; } = (short)LieuxCourriel.Brouillon;
        public string objet { get; set; }
        public List<int?> destinataires { get; set; }
    }
}