
namespace Domain_Layer.Entities;

public class User : IdentityUser
{
    public UserStatus Status { get; set; } =  UserStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
     public string? Avatar { get; set; }
    // Role relation
    public string RoleId { get; set; } = null!;
    public Role Role { get; set; } = null!;
    // Request Relation
    public ICollection<Request> Requests { get;set; } = new List<Request>();
    // Favourites Relation
    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    // Places Relation
    public ICollection<Place> Places { get;set; } = new List<Place>(); 
    // Reviews Relation
    public ICollection<Review> Rates { get; } = new List<Review>(); 
    // Registeration Relation
    public ICollection<CommericalRegisteration> Registerations { get; set; } = new List<CommericalRegisteration>();
    // ReviewReaction Relation
    public ICollection<ReviewReaction> ReviewReactions { get; set; } = new List<ReviewReaction>();
    
    // Notifications 
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
