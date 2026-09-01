using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Innovation.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Innovation.Api.Common;

public sealed class JwtTokenIssuer
{
    private readonly SymmetricSecurityKey _key;

    public JwtTokenIssuer(IConfiguration configuration)
    {
        var keyValue = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
    }

    public string Issue(UsrWt user)
    {
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.LoginName),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
