namespace PetitesPuces.Models
{
    public static class InfoCommande
    {
        public static InfoClient InfoClient { get; set; }
        public static PPVendeur Vendeur { get; set; }
        public static InfoPaiement InfoPaiement { get; set; }
        
        public static decimal? PrixLivraison { get; set; }
        public static int CodeLivraison { get; set; }
        public static Panier Panier { get; set; }
    }
}