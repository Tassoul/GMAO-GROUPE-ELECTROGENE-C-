namespace AsecnaGmao.Domain.Entities
{
    public class PieceDeRechange
    {
        public int Id { get; set; }
        public string Designation { get; set; } = string.Empty;
        public int QuantiteEnStock { get; set; }
        public int SeuilMinimum { get; set; }
    }
}
