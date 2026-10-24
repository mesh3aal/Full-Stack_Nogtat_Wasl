namespace Domain_Layer.Entities;

public class CommericalRegisteration
{
    public int Id { get; init; }
    public string CrNumber { get; set; } = null!;
    public string EntityName { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public string ImagePath { get; set; } = null!;
    // relationship
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<Place> Place { get; } = new List<Place>();
}
