namespace Domain_Layer.Entities;

public class Favourite
{
    public DateTime FavouritedAt { get; set; } = DateTime.UtcNow;
    // Relationships
    public string UserId { get; set; } = null!;
    public int PlaceId { get; set; }
    // Navigations
    public User User { get; set; } = null!;
    public Place Place { get; set; } = null!;
}
