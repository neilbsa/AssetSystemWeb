using Microsoft.AspNetCore.Identity;

namespace AssetSystem.Identity.Infrastructure.Persistence;

public sealed class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = "";

    public List<UserCompanyAccess> Companies { get; set; } = [];
}

/// <summary>Link between a user and a company they may work in. CompanyId points into the Organization module (no FK across schemas).</summary>
public sealed class UserCompanyAccess
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
}
