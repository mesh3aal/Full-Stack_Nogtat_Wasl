
using Application_Layer.Models.ResponseDTO;

namespace Application_Layer.Helper;

public class JwtHelper
{
    public static AccessTokenDto AccessToken(User user, IConfiguration conf)
    {
        var expireDate = DateTime.UtcNow.AddMinutes(conf.GetValue<int>("JwtConfig:ExpireMinutes"));
        var secretKey = conf["JwtConfig:Secret"];
        var key = Encoding.ASCII.GetBytes(secretKey!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                new Claim(ClaimTypes.Role, user.Role.Name!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("avatar",user.Avatar!)
            ]),
            Expires = expireDate,
            Issuer = conf["JwtConfig:ValidIssuer"],
            Audience = conf["JwtConfig:ValidAudiences"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AccessTokenDto
        {
            AccessToken = tokenHandler.WriteToken(token),
            ExpiresIn = (int)expireDate.Subtract(DateTime.UtcNow).TotalSeconds,
        };
    }

    public string RefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}
