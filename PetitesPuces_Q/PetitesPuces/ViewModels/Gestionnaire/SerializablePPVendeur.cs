using System.Collections.Generic;
using PetitesPuces.Models;

namespace PetitesPuces.ViewModels.Gestionnaire
{
    public class SerializablePPVendeur
    {
        private PPVendeur vendeur;
        
        public SerializablePPVendeur(PPVendeur vendeur)
        {
            this.vendeur = vendeur;
        }
        
        public long NoVendeur
        {
            get { return vendeur.NoVendeur; }
        }
        public string Prenom
        {
            get { return vendeur.Prenom; }
        }
        public string Nom
        {
            get { return vendeur.Nom; }
        }
        public string NomAffaires
        {
            get { return vendeur.NomAffaires; }
        }
        public Dictionary<string, int> typesClient { get; set; }
        public decimal TotalCommandes;
        public string Role
        {
            get { return vendeur.Role; }
        }
    }
}