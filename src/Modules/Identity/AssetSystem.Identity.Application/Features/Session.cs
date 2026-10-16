using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using Wolverine;

namespace AssetSystem.Identity.Application.Features;

public sealed record UserInfo(Guid Id, string UserName, string FullName, string Email, IReadOnlyList<string> Roles);

public sealed record CompanyInfo(Guid Id, string Code, string Name);

/// <summary>What the front end needs to bootstrap: who is signed in, where they can work and where they currently work.</summary>
public sealed record SessionResponse(
    string? Token, DateTimeOffset? ExpiresAt, UserInfo User, IReadOnlyList<CompanyInfo> Companies, Guid? CompanyId);

internal static class SessionFactory
{
    public static UserInfo ToInfo(this UserAccount a) =>
        new(a.Id, a.UserName, a.FullName, a.Email, a.Access.Roles);

    /// <summary>Administrators work in every active company; other users only in the companies assigned to them.</summary>
    public static async Task<IReadOnlyList<CompanyInfo>> AccessibleCompaniesAsync(
        this IMessageBus bus, UserAccount account, CancellationToken ct)
    {
        var companies = account.Access.IsAdministrator
            ? await bus.InvokeAsync<IReadOnlyList<CompanyDto>>(new GetActiveCompanies(), ct)
            : await bus.InvokeAsync<IReadOnlyList<CompanyDto>>(new GetCompaniesByIds(account.Access.CompanyIds), ct);

        return companies.Select(c => new CompanyInfo(c.Id, c.Code, c.Name)).ToList();
    }
}
