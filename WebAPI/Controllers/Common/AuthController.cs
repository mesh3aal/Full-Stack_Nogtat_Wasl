
using Application_Layer.Models.RequestDTO.AuthController;
using Application_Layer.Models.ResponseDTO;
using Application_Layer.Services.IdentityAndAccess;

namespace WebAPI.Controllers.Common;

[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.Register)]
    public async Task<Result> CreateUser([FromBody] RegisterDto user)
    {
        return await authService.CreateUserAsync(user);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.Login)]
    public async Task<Result<TokenDto>> Login([FromBody] LoginDto userModel)
    {
        return await authService.LoginAsync(userModel,HttpContext);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.RefreshToken)]
    public async Task<Result<TokenDto>> RefreshToken([FromBody] RefreshTokenDto token)
    {
        return await authService.RefreshToken(token);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.ForgetPassword)]
    public async Task<Result> ForgetPassword([FromBody] ForgetPasswordDto model)
    {
        return await authService.ForgetPasswordAsync(model);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.ResetPassword)]
    public async Task<Result> ResetPassword([FromBody]PasswordResetDto model)
    {
        return await authService.ResetPasswordAsync(model);
    }
    
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.IsEmailVerified)]
    public async Task<Result<bool>> IsEmailVerified(string email)
    {
        return await authService.IsEmailVerified(email);
    }
    
    [TranslateResultToActionResult]
    [HttpGet(Routing.Authentication.EmailConfirm)]
    public async Task<Result> EmailConfirm(string email, string pin)
    {
        return await authService.EmailConfirm(email, pin);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.Authentication.EmailChangeConfirm)]
    public async Task<Result> EmailConfirm(string userId, string email, string token)
    {
        return await authService.EmailConfirm(userId,email,token);
    }

    [TranslateResultToActionResult]
    [HttpGet(Routing.Authentication.ResendEmailConfirmation)]
    public async Task<Result> ResendEmailConfirmation(string email)
    {
        return await authService.ResendEmailConfirmation(email);
    }
    [TranslateResultToActionResult]
    [HttpPost(Routing.Authentication.Logout)]
    public async Task<Result> Logout()
    {
        return await authService.Logout();
    }
}
