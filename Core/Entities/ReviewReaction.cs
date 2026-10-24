
namespace Domain_Layer.Entities;

public class ReviewReaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // review
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    // user
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public ReactionType Type { get; set; }
}
