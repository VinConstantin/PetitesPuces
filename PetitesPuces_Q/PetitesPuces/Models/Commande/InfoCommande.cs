namespace PetitesPuces.Models
{
    public static class InfoCommande
    {
        public static InfoClient InfoClient { get; set; }
        public static InfoPaiement InfoPaiement { get; set; }
        
        public static decimal? PrixLivraison { get; set; }
        public static Panier Panier { get; set; }

        public static void SetInfoClient(InfoClient info)
        {
            InfoClient = info;
        }
        public static void SetInfoPaiement(InfoPaiement info)
        {
            InfoPaiement = info;
        }

    }
}