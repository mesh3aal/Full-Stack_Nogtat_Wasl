using Application_Layer.Models.ResponseDTO.ReviewReactionController;

namespace Application_Layer.Models.ResponseDTO.ReviewController;

public record ReviewResponse
{
    public int ReviewId { get; set; }
    public string PlaceName { get; set; } = null!;
    public sbyte RateValue { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserInfoRate User { get; set; } = null!;
    public ReviewReactionStatus ReactionType { get; set; }
    public ReviewReactionCount ReactionCount { get; set; } = null!;
}
