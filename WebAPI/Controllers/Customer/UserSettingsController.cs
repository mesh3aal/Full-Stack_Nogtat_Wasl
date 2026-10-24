
using Application_Layer.Models.RequestDTO.UsersettingsController;
using Application_Layer.Models.ResponseDTO.UsersettingsController;
using Application_Layer.Services.IdentityAndAccess;

namespace WebAPI.Controllers.Customer;

[ApiController]
public class UserSettingsController(IUserSettingsService userSettingsService) : ControllerBase
{
    [TranslateResultToActionResult]
    [HttpPost(Routing.CustomerSettings.ChangePassword)]
    public async Task<Result<string>> ChangePassword([FromBody] ChangePasswordDto model)
    {
        return await userSettingsService.ChangePasswordAsync(model);
    }
    [TranslateResultToActionResult]
    [HttpGet(Routing.CustomerSettings.GetProfileInfo)]
    public async Task<Result<UserProfileDtoResponse>> GetProfileInfo()
    {
        return await userSettingsService.GetProfileInfo();
    }

    [TranslateResultToActionResult]
    [HttpPatch(Routing.CustomerSettings.ChangeProfile)]
    public async Task<Result<string>> ChangeProfile(IFormFile? profileImage, string? username, string? email)
    {
        return await userSettingsService.ChangeProfile(profileImage,username,email);
    }

    [HttpPost(Routing.CustomerSettings.Deactivate)]
    [TranslateResultToActionResult]
    public async Task<Result> Deactivate()
    {
        return await userSettingsService.Deactivate();
    }
    [HttpPost(Routing.CustomerSettings.Reactivate)]
    [TranslateResultToActionResult]
    public async Task<Result> Reactivate(string email)
    {
        return await userSettingsService.Reactivate(email);
    }
}
