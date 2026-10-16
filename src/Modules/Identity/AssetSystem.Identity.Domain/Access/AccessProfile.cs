using AssetSystem.SharedKernel.Domain;
using AppRoles = AssetSystem.SharedKernel.Abstractions.Roles;

namespace AssetSystem.Identity.Domain.Access;

/// <summary>
/// What a user is allowed to do (roles) and where (companies). Business rules:
/// at least one known role; administrators reach every company, everyone else needs at least one assigned company.
/// </summary>
public sealed class AccessProfile : ValueObject
{
    public static readonly IReadOnlyList<string> KnownRoles = [AppRoles.Administrator, AppRoles.Common];

    private AccessProfile(IReadOnlyList<string> roles, IReadOnlyList<Guid> companyIds)
    {
        Roles = roles;
        CompanyIds = companyIds;
    }

    public IReadOnlyList<string> Roles { get; }
    public IReadOnlyList<Guid> CompanyIds { get; }

    public bool IsAdministrator => Roles.Contains(AppRoles.Administrator);

    public static AccessProfile Create(IEnumerable<string>? roles, IEnumerable<Guid>? companyIds)
    {
        var roleList = (roles ?? []).Select(r => r.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var companyList = (companyIds ?? []).Distinct().ToList();

        Guard.That(roleList.Count > 0, "access.role_required", "Assign at least one role.");
        Guard.That(roleList.All(r => KnownRoles.Contains(r, StringComparer.OrdinalIgnoreCase)),
            "access.unknown_role", "One or more roles are not recognised.");

        var normalised = roleList
            .Select(r => KnownRoles.First(k => string.Equals(k, r, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var profile = new AccessProfile(normalised, companyList);
        Guard.That(profile.IsAdministrator || companyList.Count > 0,
            "access.company_required", "Assign at least one company (administrators are exempt).");
        return profile;
    }

    /// <summary>Rebuilds a profile from stored data without re-validating (rules may have changed since it was saved).</summary>
    public static AccessProfile Restore(IEnumerable<string> roles, IEnumerable<Guid> companyIds) =>
        new(roles.ToList(), companyIds.ToList());

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        foreach (var role in Roles.Order()) yield return role;
        foreach (var company in CompanyIds.Order()) yield return company;
    }
}
