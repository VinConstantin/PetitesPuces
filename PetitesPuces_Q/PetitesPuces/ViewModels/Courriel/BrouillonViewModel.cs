using System.Collections.Generic;

namespace PetitesPuces.ViewModels.Courriel
{
    public class BrouillonViewModel
    {
        public long? NoMsg { get; set; }
        public string DescMsg { get; set; }
        public short? Lieu { get; set; }
        public string objet { get; set; }
        public List<int> destinataires { get; set; }
    }
}