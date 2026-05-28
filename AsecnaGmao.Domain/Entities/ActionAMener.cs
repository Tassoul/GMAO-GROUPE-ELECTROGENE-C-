namespace AsecnaGmao.Domain.Entities
{
    public class ActionAMener
    {
        public int Id { get; set; }
        public int BonInterventionId { get; set; }
        public string DescriptionAction { get; set; } = string.Empty;
        public bool EstFaite { get; set; }
    }
}
