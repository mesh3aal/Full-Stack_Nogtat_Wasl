namespace Application_Layer.Models.ResponseDTO;

public class AccessTokenDto
{
    public string AccessToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
}
