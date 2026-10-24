using Application_Layer.Models.RequestDTO.AuthController;
using Application_Layer.Models.ResponseDTO;
using Application_Layer.Services.CommunicationsAndNotifications;
using Application_Layer.Services.InfrastructureAndUtilities;

namespace Application_Layer.Services.IdentityAndAccess;

public class AuthService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    ApplicationDbContext context,
    IConfiguration conf,
    IUrlProvider urlProvider,
    IEmailQueue emailQueue,
    ICurrentUserService currentUser,
    IHttpContextAccessor http)
    : IAuthService
{ 
    public async Task<Result> CreateUserAsync(RegisterDto user)
    {
        var baseUrl = urlProvider.GetBaseUrl();

        // Getting RoleName
        var roleName = user.UserType.ToString();
        var role = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null) return Result.NotFound("لم يتم إجاد الدور المطلوب");
        var roleId = role.Id;

        // Creating User
        var createdUser = new User
        {
            UserName = user.Username,
            Email = user.Email,
            RoleId = roleId,
            Avatar = new Uri(new Uri(baseUrl),"images/global/default.jpg").ToString(),
            Status = UserStatus.Active,
        };

        var result = await userManager.CreateAsync(createdUser, user.Password);
        if (!result.Succeeded)
        {
            return Result.Invalid(result.Errors.Select(x => new ValidationError{ErrorMessage = x.Description, ErrorCode = x.Code}));
        }

        await userManager.AddToRoleAsync(createdUser, roleName);
        emailQueue.Enqueue(async sp =>
        {
            var emailSenderService = sp.GetRequiredService<IEmailSender>();
            await emailSenderService.SendConfirmationEmailAsync(createdUser);
        });

        return Result.Success();
    }

    public async Task<Result<TokenDto>> LoginAsync(LoginDto model, HttpContext http)
    {
        var user = await userManager.Users
            .Include(r => r.Role)
            .FirstOrDefaultAsync(x => x.Email == model.Email);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        {
            return Result<TokenDto>.NotFound("اسم المستخدم او كلمة المرور غير صحيحة");
        }
        
        var token = JwtHelper.AccessToken(user, conf);
        
        var refreshExpire = DateTime.UtcNow.AddDays(7);
        
        var refreshToken = new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = new JwtHelper().RefreshToken(),
            ExpiresOnUtc = refreshExpire
        };
        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return Result<TokenDto>.Success(new TokenDto
        {
            RoleName = user.Role.Name!,
            AccessToken = token.AccessToken,
            AccessTokenExpiresInSec = token.ExpiresIn,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresDateTime = refreshExpire
        });
    }

    public async Task<Result<TokenDto>> RefreshToken(RefreshTokenDto model)
    {
        var storedToken = await context.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
        
        if (storedToken is null || storedToken.ExpiresOnUtc <= DateTime.UtcNow)
        {
            return Result.Invalid(new ValidationError{ErrorMessage = "لقد انتهى وقت التوكين، او تم عمل تسجيل خروج", ErrorCode = "خطاء بالتحقق من التوكين"});
        }

        var token = JwtHelper.AccessToken(storedToken.User, conf);
        storedToken.Token = new JwtHelper().RefreshToken();
        var refreshExpire = DateTime.UtcNow.AddDays(7);
        storedToken.ExpiresOnUtc = refreshExpire;

        await context.SaveChangesAsync();
        return Result<TokenDto>.Success(new TokenDto
        {
            RoleName = storedToken.User.Role.Name!,
            AccessToken = token.AccessToken,
            AccessTokenExpiresInSec = token.ExpiresIn,
            RefreshToken = storedToken.Token,
            RefreshTokenExpiresDateTime = refreshExpire
        });
    }

    // طلب نسيت كلمة المرور إلى الايميل
    public async Task<Result> ForgetPasswordAsync(ForgetPasswordDto model)
    {
        var baseUrl = urlProvider.GetBaseUrl();
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return Result.NotFound("لم يتم إجاد المستخدم المطلوب");
        }
        
        emailQueue.Enqueue(async sp =>
        {
            var emailSenderService = sp.GetRequiredService<IEmailSender>();
            await emailSenderService.SendForgotPasswordEmailAsync(user, baseUrl);
        });
    
        return Result.Success();
    }
    // إعادة تعين كلمة المرور من الطلب بالايميل.
    public async Task<Result> ResetPasswordAsync(PasswordResetDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return Result.NotFound("لم يتم إيجاد المستخدم المطلوب");
        }

        // 1. التحقق من أن الـ TOTP المكون من 6 أرقام صحيح ولم تنتهِ صلاحيته
        var isValidPin = await userManager.VerifyUserTokenAsync(user, "Phone", "ResetPasswordByPin", model.Pin);
        if (!isValidPin)
        {
            return Result.Invalid(new List<ValidationError> 
            { 
                new ValidationError { ErrorMessage = "الرمز المدخل غير صحيح أو انتهت صلاحيته", ErrorCode = "InvalidToken" } 
            });
        }

        // 2. إذا كان الرمز صحيحاً، نقوم بإزالة كلمة المرور القديمة وتعيين الجديدة مباشرة
        // (لأن الرمز تم التحقق منه ونجحنا في التأكد من هوية المستخدم)
        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return Result.Invalid(new ValidationError
            {
                ErrorCode = "400",
                ErrorMessage = "لم نتمكن من ازالة كلمة المرور القديمة"
            });
        }

        var addResult = await userManager.AddPasswordAsync(user, model.Password);
        if (!addResult.Succeeded)
        {
            return Result.Invalid(new ValidationError
            {
                ErrorCode = "400",
                ErrorMessage = "لم نتمكن من إضافة كلمة المرور الجديدة"
            });
        }

        return Result.Success();
    }
    

    public async Task<Result<bool>> IsEmailVerified(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user == null) return Result.NotFound("لم يتم إجاد المستخدم المطلوب");
        
        var result = await userManager.IsEmailConfirmedAsync(user);
        return Result<bool>.Success(result);
    }
    public async Task<Result> EmailConfirm(string email, string pin)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return Result.NotFound("لم يتم إجاد المستخدم المطلوب");
        
        // إذا كان الحساب مفعل من اول
        if (user.EmailConfirmed)
            return Result.Conflict("هذا الحساب موثق بالفعل.");
        
        // 1. التحقق من صحة الرمز المكون من 6 أرقام الخاص بتفعيل الإيميل
        var isValidToken = await userManager.VerifyUserTokenAsync(user, "Phone", "ConfirmEmailByPin", pin);

        if (!isValidToken)
        {
            return Result.Invalid(new List<ValidationError> 
            { 
                new ValidationError { ErrorMessage = "الرمز المدخل غير صحيح أو انتهت صلاحيته", ErrorCode = "InvalidPin" } 
            });
        }

        // 2. إذا كان الرمز صحيحاً، نقوم بتحديث حالة المستخدم إلى "مفعّل" داخلياً
        user.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Invalid(result.Errors.Select(x => new ValidationError { ErrorMessage = x.Description, ErrorCode = x.Code }));
        }

        return Result.Success();
    }
    public async Task<Result> EmailConfirm(string userId, string email, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result.NotFound("لم يتم إجاد المستخدم المطلوب");

        var result = await userManager.ChangeEmailAsync(user, email, token);

        if (!result.Succeeded)
        {
            return Result.Error("حدث خطاء لم ننجح في تغير البريد الألكتروني");
        }
        await userManager.UpdateSecurityStampAsync(user);

        return Result.Success();
    }

    public async Task<Result> ResendEmailConfirmation(string email)
    {
        var baseUrl = urlProvider.GetBaseUrl();
        var user = await userManager.FindByEmailAsync(email);
  
        if (user == null) 
            return Result.Success(); 

        if (user.EmailConfirmed)
        {
            return Result.Conflict("هذا الحساب موثق بالفعل.");
        }
        
        emailQueue.Enqueue(async sp =>
        {
            var emailSenderService = sp.GetRequiredService<IEmailSender>();
            await emailSenderService.SendConfirmationEmailAsync(user);
        });
        return Result.Success();
    }

    public async Task<Result> Logout()
    {
        var user = await currentUser.GetUserAsync();

        if (user != null)
        {
            var refreshTokensInDb = context.RefreshTokens.Where(x => x.UserId == user.Id);
            context.RefreshTokens.RemoveRange(refreshTokensInDb);
            await context.SaveChangesAsync();
        }

        http.HttpContext?.Response.Cookies.Delete("accessToken");
        http.HttpContext?.Response.Cookies.Delete("refreshToken");

        return Result.NoContent();
    }
}
