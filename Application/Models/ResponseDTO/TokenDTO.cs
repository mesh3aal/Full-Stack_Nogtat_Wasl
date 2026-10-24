namespace Application_Layer.Models.ResponseDTO;

public record TokenDto
{
    public string RoleName { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public int AccessTokenExpiresInSec { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresDateTime { get; set; }
}
