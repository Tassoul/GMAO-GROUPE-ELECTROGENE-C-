using System;
using System.Collections.Generic;

namespace AsecnaGmao.Domain.Entities
{
    public class BonIntervention
    {
        public int Id { get; set; }
        public int GroupeElectrogeneId { get; set; }
        public int UtilisateurId { get; set; }
        public DateTime DateHeure { get; set; }
        public string TypeIntervention { get; set; } = string.Empty; // "Préventif", "Curatif"
        public string Commentaire { get; set; } = string.Empty;
        public string OrigineDataSource { get; set; } = string.Empty; // "Saisie_Directe", "Source_Papier"
        public List<ActionAMener> Actions { get; set; } = new();
    }
}
