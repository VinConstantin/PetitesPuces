namespace PetitesPuces.Models
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public double Price { get; set; }

        public Product(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}