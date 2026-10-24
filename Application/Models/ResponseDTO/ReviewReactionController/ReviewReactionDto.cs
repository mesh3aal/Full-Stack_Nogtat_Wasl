
namespace Application_Layer.Models.ResponseDTO.ReviewReactionController;

public record ReviewReactionDto
{
    public ReviewReactionStatus State { get; set; }
    public int ReviewId { get; set; }
    public int LikesCount { get; set; }
    public int DislikesCount { get; set; }
};
