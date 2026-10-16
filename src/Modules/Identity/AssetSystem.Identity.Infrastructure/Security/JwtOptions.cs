namespace AssetSystem.Identity.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string Section = "Jwt";

    public string Issuer { get; set; } = "assetsystem";
    public string Audience { get; set; } = "assetsystem-web";

    /// <summary>HMAC signing key, at least 32 characters. Supply through configuration/secrets, never source control.</summary>
    public string SigningKey { get; set; } = "";

    public int LifetimeMinutes { get; set; } = 480;
}
