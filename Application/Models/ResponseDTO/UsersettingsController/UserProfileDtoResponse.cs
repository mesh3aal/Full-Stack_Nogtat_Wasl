namespace Application_Layer.Models.ResponseDTO.UsersettingsController;

public record UserProfileDtoResponse()
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
}