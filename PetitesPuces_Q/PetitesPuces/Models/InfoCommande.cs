namespace PetitesPuces.Models
{
    public static class InfoCommande
    {
        private static InfoClient Info { get; set; }
        
        public static string NoCarteCredit { get; set; } 
        public static string DateExpirationCarteCredit { get; set; } 
        public static string NoSecuriteCarteCredit { get; set; }

        public static void setInfoClient(InfoClient info)
        {
            Info = info;
        }
    }
}