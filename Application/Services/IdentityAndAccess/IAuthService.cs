
using Application_Layer.Models.RequestDTO.AuthController;
using Application_Layer.Models.ResponseDTO;

namespace Application_Layer.Services.IdentityAndAccess;

public interface IAuthService
{
    Task<Result> CreateUserAsync(RegisterDto user);
    Task<Result<TokenDto>> LoginAsync(LoginDto model, HttpContext http);
    Task<Result<TokenDto>> RefreshToken(RefreshTokenDto model);
    Task<Result> ForgetPasswordAsync(ForgetPasswordDto model);
    Task<Result> ResetPasswordAsync(PasswordResetDto model);
    Task<Result<bool>> IsEmailVerified(string email);
    Task<Result> EmailConfirm(string email, string pin);
    Task<Result> EmailConfirm(string UserId, string email, string Token);
    Task<Result> ResendEmailConfirmation(string email);
    Task<Result> Logout();
}
