using Application_Layer.Models.RequestDTO.UsersettingsController;
using Application_Layer.Models.ResponseDTO.UsersettingsController;

namespace Application_Layer.Services.IdentityAndAccess;

public interface IUserSettingsService
{
    Task<Result<string>> ChangePasswordAsync(ChangePasswordDto model);
    Task<Result<UserProfileDtoResponse>> GetProfileInfo();
    Task<Result<string>> ChangeProfile(IFormFile? profileImage, string? username, string? email);
    Task<Result> Deactivate();
    Task<Result> Reactivate(string email);
}
