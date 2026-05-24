using System;

namespace AsecnaGmao.Domain.Entities
{
    public class LogTransaction
    {
        public int Id { get; set; }
        public string TableNom { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
        public string ValeursAnciennes { get; set; } = string.Empty; // Format JSON
        public string ValeursNouvelles { get; set; } = string.Empty; // Format JSON
        public int UtilisateurId { get; set; }
        public DateTime DateHeure { get; set; }
    }
}
