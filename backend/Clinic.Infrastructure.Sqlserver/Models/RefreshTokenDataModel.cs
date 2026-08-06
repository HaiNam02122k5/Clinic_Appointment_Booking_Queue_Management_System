namespace Clinic.Infrastructure.Sqlserver.Models
{
    public class RefreshTokenDataModel
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Navigation
        public UserDataModel User { get; set; }
    }
}
