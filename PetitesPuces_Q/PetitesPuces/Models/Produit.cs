using System.ComponentModel;

namespace PetitesPuces.Models
{
    public class Produit
    {
        [DisplayName("No de produit")]
        public int Id { get; private set; }

        [DisplayName("Nom")]
        public string Name { get; private set; }

        [DisplayName("Prix unitaire")]
        public double Price { get; set; }

        public Produit(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}