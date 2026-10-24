
namespace Domain_Layer.Entities;

public class Review
{
    public int ReviewId { get; set; }
    [Range(1,5, ErrorMessage = "لايمكن ان يكون اكثر من 5 و اقل من 1")]
    public sbyte RateValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; }
    //Places Relationship M:1
    public int PlaceId { get; set; }
    public Place Place { get; set; } = null!;
    // User Relationship
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    // Likes Relationship
    public ICollection<ReviewReaction> ReviewReactions { get; set; } = new List<ReviewReaction>();
}   

