using System.Net;
using System.Security.Claims;
using Scalar.AspNetCore;

namespace WebAPI;

public static class ApiDependencyInjection
{
    public static void AddCorsReact(this IServiceCollection services)
    {
        services.AddCors(policy =>
        {
            policy.AddDefaultPolicy(p =>
            {
                p.WithOrigins("http://localhost:3000", "https://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });
    }
    public static void AddRepresentationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        AddJwt(services, configuration);
        AddControllerConfigurations(services);
    }
    
    private static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration.GetValue<string>("JwtConfig:Secret");

        var key = Encoding.ASCII.GetBytes(secretKey!);
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer =  configuration["JwtConfig:ValidIssuer"],
                    ValidAudience =  configuration["JwtConfig:ValidAudiences"],
                    ValidateLifetime =  true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                        if (!string.IsNullOrEmpty(accessToken)) ctx.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void AddControllerConfigurations(this IServiceCollection services)
    {
        services.AddControllers(opt =>
        {
            opt.AddDefaultResultConvention();
            opt.AddResultConvention(resultStatusMap => resultStatusMap
                .AddDefaultMap()
                .For(ResultStatus.NotFound,
                    HttpStatusCode.NotFound,
                    resultStatusOptions => resultStatusOptions
                        .With((controller, result) => new ProblemDetails
                            {
                                Title = "لم يتم إجاد المصدر",
                                Status = 404,
                                Detail = result.Errors.FirstOrDefault()
                            }
                        )
                )
                // 400
                .For(ResultStatus.Invalid,
                    HttpStatusCode.BadRequest,
                    options => options.With((controller, result) =>
                        new ValidationProblemDetails(
                            result.ValidationErrors
                                .GroupBy(x => x.Identifier ?? "Error")
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(x => x.ErrorMessage).ToArray()
                                ))
                        {
                            Title = "البيانات غير صالحة",
                            Status = 400,
                            Detail = "يوجد أخطاء في البيانات المدخلة"
                        }))

                // 401
                .For(ResultStatus.Unauthorized,
                    HttpStatusCode.Unauthorized,
                    options => options.With((controller, result) =>
                        new ProblemDetails
                        {
                            Title = "الرجاء تسجيل الدخول",
                            Status = 401,
                            Detail = result.Errors.FirstOrDefault()
                        }))

                // 403
                .For(ResultStatus.Forbidden,
                    HttpStatusCode.Forbidden,
                    options => options.With((controller, result) =>
                        new ProblemDetails
                        {
                            Title = "غير مصرح لك بالوصول إلى المصدر",
                            Status = 403,
                            Detail = result.Errors.FirstOrDefault()
                        }))

                // 500
                .For(ResultStatus.Error,
                    HttpStatusCode.InternalServerError,
                    options => options.With((controller, result) =>
                        new ProblemDetails
                        {
                            Title = "حدث خطأ داخلي",
                            Status = 500,
                            Detail = result.Errors.FirstOrDefault()
                        }))

                // 409
                .For(ResultStatus.Conflict,
                    HttpStatusCode.Conflict,
                    options => options.With((controller, result) =>
                        new ProblemDetails
                        {
                            Title = "تعارض في البيانات",
                            Status = 409,
                            Detail = result.Errors.FirstOrDefault()
                        }))
            );
        }).AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
    }
    
    public static void UseRepresentation(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("docs", opt =>
        {
            opt.WithOpenApiRoutePattern("/openapi/v1.json");
            opt.WithTitle("Documentation");
        });
    }
}
