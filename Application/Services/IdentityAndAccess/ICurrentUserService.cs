
namespace Application_Layer.Services.IdentityAndAccess;

public interface ICurrentUserService
{
    Task<User?> GetUserAsync();
    string? GetUserId();
    bool IsUserInRole(string role);
    List<string> GetUserRole();
    bool IsAuthenticatedAsync();
}
