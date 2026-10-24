namespace Application_Layer.Models.ResponseDTO.ReviewController;

public record IntractionTypes
{
    public int ReviewId { get; set; }
    public ReactionType ReactionType { get; set; }
}