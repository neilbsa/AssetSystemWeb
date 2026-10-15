using System.Security.Claims;
using System.Text;
using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.SharedKernel.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AssetSystem.Identity.Infrastructure.Security;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options, TimeProvider clock) : ITokenIssuer
{
    private readonly JwtOptions _options = options.Value;

    public IssuedToken Issue(UserAccount account, Guid? companyId)
    {
        var now = clock.GetUtcNow();
        var expires = now.AddMinutes(_options.LifetimeMinutes);

        var claims = new List<Claim>
        {
            new("sub", account.Id.ToString()),
            new("name", account.UserName),
        };
        claims.AddRange(account.Access.Roles.Select(r => new Claim("role", r)));
        if (companyId is not null) claims.Add(new Claim(AppClaims.CompanyId, companyId.Value.ToString()));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = now.UtcDateTime,
            Expires = expires.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)), SecurityAlgorithms.HmacSha256)
        };

        return new IssuedToken(new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}
