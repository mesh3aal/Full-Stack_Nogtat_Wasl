
namespace Domain_Layer.Entities;

public class Place
{
    public int PlaceId { get; set; }
    public string PlaceName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Email { get; set; }
    public PlaceStatus State { get; set; } =  PlaceStatus.Active;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    public List<WeekDays> UnavailableAt { get; set; } = [WeekDays.الجمعة];
    

    //Request Relationship 1:1
    public int RequestId { get; set; }
    public Request Request { get; set; } = null!;
    // Rates Relationship 1:M
    public ICollection<Review> Rates { get; set; } = new List<Review>();
    // Favourites Relationship 1:M
    public List<Favourite> FavouritedBy { get; set; } = new List<Favourite>();

    // CommericalRegisteration Relationship
    public int CommericalRegisterationId { get; set; }
    public CommericalRegisteration CommericalRegisteration { get; set; } = null!;
    
    
    // PhoneNumber Relationship
    public ICollection<PhoneNumber> Phones { get; set; } = new List<PhoneNumber>();
    

    // Images Relationship ?1:M 
    public ICollection<PlaceImage> Images { get; set; } = new List<PlaceImage>();
    
    
    // Sub-Categories RelationShip ?M:1
    public int SubCategoryId { get; set; }
    public SubCategory SubCategory { get; set; } = null!;

// User Relationship M:?1
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    
    // Location relationship
    public Location Location { get; set; } = null!;
    
    
    
  
}
