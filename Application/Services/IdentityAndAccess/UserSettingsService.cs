using Application_Layer.Models.RequestDTO.UsersettingsController;
using Application_Layer.Models.ResponseDTO.UsersettingsController;
using Application_Layer.Services.CommunicationsAndNotifications;
using Application_Layer.Services.InfrastructureAndUtilities;

namespace Application_Layer.Services.IdentityAndAccess;

public class UserSettingsService(
    UserManager<User> userManager,
    ICurrentUserService currentUser,
    ApplicationDbContext context,
    IWebHostEnvironment env,
    IUrlProvider urlProvider,
    IEmailQueue emailQueue)
    : IUserSettingsService
{
    public async Task<Result<string>> ChangePasswordAsync(ChangePasswordDto model)
    {
        var user = await currentUser.GetUserAsync();
        if (user == null)
        {
            return Result<string>.Unauthorized();
        }
        var result = await userManager.ChangePasswordAsync(user,model.OldPassword,model.NewPassword);
        
        if (!result.Succeeded) 
        {
            var validationErrors = result.Errors
                .Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description })
                .ToList();

            return Result.Invalid(validationErrors);
        }
        return Result.Success();
    }
    public async Task<Result<UserProfileDtoResponse>> GetProfileInfo()
    {
        var user = await currentUser.GetUserAsync();
        if (user == null)
        {
            return Result<UserProfileDtoResponse>.Unauthorized();
        }

        var data = await context.Users.Where(x => x.Id == user.Id).Select(x => new UserProfileDtoResponse
        {
            Username = x.UserName,
            Email = x.Email,
            ProfileImage = x.Avatar
        }).FirstAsync();
        return Result<UserProfileDtoResponse>.Success(data);
    }
    public async Task<Result<string>> ChangeProfile(IFormFile? profileImage, string? username, string? email)
    {
        var baseUrl = urlProvider.GetBaseUrl();
        var user = await currentUser.GetUserAsync();
        if (user == null)
        {
            return Result<string>.Unauthorized();
        }
        // if user is not signed in (up)

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailExists = context.Users.Any(x => x.Email == email);
            if (emailExists)
            {
                return Result<string>.Conflict("الايميل الجديد موجود مسبقاً كمستخدم");
            }
            var token = await userManager.GenerateChangeEmailTokenAsync(user, email);

            emailQueue.Enqueue(async sp =>
            {
                var emailSenderService = sp.GetRequiredService<IEmailSender>();
                await emailSenderService.SendConfirmationEmailAsync(user, user.Id, email,token,baseUrl);
            });
        }
        // changing email process (up)


        if (!string.IsNullOrWhiteSpace(username))
        {
            var usernameExists = context.Users.Any(x => x.UserName == username);
            if (usernameExists)
            {
                return Result<string>.Conflict("اسم المستخدم الجديد موجود مسبقاً");
            }
            var setUsernameResult = await userManager.SetUserNameAsync(user, username);

            if (!setUsernameResult.Succeeded)
            {
                return Result<string>.Invalid(setUsernameResult.Errors.Select(e => new ValidationError { Identifier = e.Code, ErrorMessage = e.Description }).ToList());
            }
        }

        if (profileImage != null)
        {
            long maxFileSize = 1 * 1024 * 1024;

            if (profileImage.Length > maxFileSize)
            {
                return Result<string>.Invalid(new ValidationError("حجم الملف اكبر من 1 ميقا، الرجاء اختيار صورة بحجم اقل من ذلك"));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(profileImage.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Result<string>.Invalid(new ValidationError("نوع الملف ليس من الانواع التالية jpg او jpeg, او png."));
            }

            var uploadsFolder = Path.Combine(env.WebRootPath, $"images/{user.Id}");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                try
                {
                    var oldFileName = Path.GetFileName(user.Avatar);
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                    if (File.Exists(oldFilePath) && !oldFilePath.Contains("default.jpg"))
                    {
                        File.Delete(oldFilePath);
                    }
                }
                catch (Exception e)
                {
                    return Result<string>.Invalid(new ValidationError(e.Message));
                }
            }

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(fileStream);
            }


            var avatarUrl = $"{baseUrl}/images/{user.Id}/{uniqueFileName}";

            user.Avatar = avatarUrl;
        }
        await context.SaveChangesAsync();
        return Result.Success();
    }
    public async Task<Result> Deactivate()
    {
        var baseUrl = urlProvider.GetBaseUrl();
        var user = await currentUser.GetUserAsync();
        if (user == null)
        {
            return Result.Unauthorized();
        }

        if (user.DeletedAt != null)
        {
            return Result.Conflict("هذا الحساب معطل بالفعل.");
        }
        
        user.DeletedAt = DateTime.UtcNow;
        user.Status = UserStatus.PendingDeletion;
        
        
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result.Invalid(new ValidationError("لم يستطع الخادم حفظ البيانات الجديدة"));
        }
        emailQueue.Enqueue(async sp =>
        {
            var emailSenderService = sp.GetRequiredService<IEmailSender>();
            await emailSenderService.DeactivateAccountAsync(user, baseUrl);
        });
        
        return Result.Success();
    }

    public async Task<Result> Reactivate(string email)
    {
        var baseUrl = urlProvider.GetBaseUrl();
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.Unauthorized();
        }
        if(user.Status == UserStatus.Active)
        {
            return Result.Conflict("الحساب مفعل، لاحاجة لإعادة تفعيل الحساب");
        }
        user.DeletedAt = null;
        user.Status = UserStatus.Active;
        await context.SaveChangesAsync();
        
        emailQueue.Enqueue(async sp =>
        {
            var emailSenderService = sp.GetRequiredService<IEmailSender>();
            await emailSenderService.ReactivateAccountAsync(user, baseUrl);
        });
        
        return Result.Success();
    }
}
