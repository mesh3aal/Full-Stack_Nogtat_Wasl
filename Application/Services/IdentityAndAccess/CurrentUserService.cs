namespace Application_Layer.Services.IdentityAndAccess;

public class CurrentUserService(IHttpContextAccessor contextAccessor, UserManager<User> userManager)
    : ICurrentUserService
{
    public async Task<User?> GetUserAsync()
    {
        var claimsPrincipal = contextAccessor.HttpContext?.User;

        if (claimsPrincipal == null)
            return null;

        return await userManager.GetUserAsync(claimsPrincipal);
    }

    public string? GetUserId()
    {
        return contextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;
    }

    public bool IsUserInRole(string role)
    {
        return contextAccessor.HttpContext!.User.IsInRole(role);
    }

    public List<string>? GetUserRole()
    {
        var roles = contextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value).ToList();
        return roles!;
    }

    public bool IsAuthenticatedAsync()
    {
        var claimPrincipals =contextAccessor.HttpContext;
        if (claimPrincipals == null) return false;
        var user = claimPrincipals.User;
        return user.Identity!.IsAuthenticated;
    }
}
