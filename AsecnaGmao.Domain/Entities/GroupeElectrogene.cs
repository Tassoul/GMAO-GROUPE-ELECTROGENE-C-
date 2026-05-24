namespace AsecnaGmao.Domain.Entities
{
    public class GroupeElectrogene
    {
        public int Id { get; set; }
        public string ReferenceAsecna { get; set; } = string.Empty;
        public string Modele { get; set; } = string.Empty;
        public double CompteurHeures { get; set; }
        public double DerniereMaintenanceHeures { get; set; }
        public double CycleSeuilHeures { get; set; } // Ex: 250, 500, 1000
        public bool EstSupprime { get; set; }

        public bool NecessiteMaintenance()
        {
            return (CompteurHeures - DerniereMaintenanceHeures) >= CycleSeuilHeures;
        }
    }
}
