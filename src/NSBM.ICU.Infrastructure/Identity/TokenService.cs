using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Infrastructure.Identity;

public sealed class TokenService(IOptions<JwtOptions> options, IDateTime clock) : ITokenService
{
    private readonly JwtOptions _jwt = options.Value;

    public AccessToken CreateToken(AuthenticatedUser user)
    {
        var expiresAt = clock.UtcNow.AddMinutes(_jwt.ExpiryMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret)),
            SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
        ];

        var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims,
            notBefore: clock.UtcNow, expires: expiresAt, signingCredentials: credentials);

        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
