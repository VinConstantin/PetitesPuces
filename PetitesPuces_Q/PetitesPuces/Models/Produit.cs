using System.ComponentModel;

namespace PetitesPuces.Models
{
    public class Product
    {
        [DisplayName("No de produit")]
        public int Id { get; private set; }

        [DisplayName("Nom")]
        public string Name { get; private set; }

        [DisplayName("Prix unitaire")]
        public double Price { get; set; }

        public Product(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}