namespace AsecnaGmao.Domain.Entities
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Administrateur", "Technicien"
    }
}
