
namespace Application_Layer.Services.InfrastructureAndUtilities;

public class UrlProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IUrlProvider
{
    public string GetBaseUrl()
    {
        var request = httpContextAccessor.HttpContext?.Request;
    
        if (request != null)
        {
            return $"{request.Scheme}://{request.Host}";
        }
        
        /*var baseUrl = configuration["BaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
        {
            return baseUrl;
        }*/

        throw new InvalidOperationException("Cannot determine BaseUrl. HTTP Context is null and 'BaseUrl' is not set in configuration.");
    }
}
