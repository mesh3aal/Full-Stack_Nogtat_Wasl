namespace Application_Layer.Models.ResponseDTO.ReviewController;

public record UserInfoRate
{
    public required string UserName { get; set; } = null!;
    public required string UserAvatar { get; set; } = null!;
}