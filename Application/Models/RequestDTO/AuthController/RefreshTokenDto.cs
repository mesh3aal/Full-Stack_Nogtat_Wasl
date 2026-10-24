namespace Application_Layer.Models.ReciveDTOs.AuthController;

public record RefreshTokenDto
{
    [Required(ErrorMessage = "لايمكن ان يكون فارغاً")]
    public string RefreshToken { get; set; } = null!;
}