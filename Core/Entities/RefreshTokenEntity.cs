namespace Domain_Layer.Entities;

public record RefreshTokenEntity
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime ExpiresOnUtc { get; set; }
    public User User { get; set; } = null!;
}
