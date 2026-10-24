namespace Application_Layer.Models.ResponseDTO.ReviewReactionController;

public record ReviewReactionCount
{
    public int Likes { get; set; }
    public int Dislikes { get; set; }
}