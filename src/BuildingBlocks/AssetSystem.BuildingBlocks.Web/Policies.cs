using AssetSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.BuildingBlocks.Web;

public static class Policies
{
    /// <summary>Signed in AND has picked the company they are working in (JWT carries company_id).</summary>
    public const string CompanySelected = nameof(CompanySelected);

    public const string Administrator = nameof(Administrator);

    /// <summary>Administrator who has also picked a company.</summary>
    public const string CompanyAdministrator = nameof(CompanyAdministrator);

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(CompanySelected, p => p.RequireAuthenticatedUser().RequireClaim(AppClaims.CompanyId))
            .AddPolicy(Administrator, p => p.RequireAuthenticatedUser().RequireRole(Roles.Administrator))
            .AddPolicy(CompanyAdministrator, p => p.RequireAuthenticatedUser().RequireRole(Roles.Administrator).RequireClaim(AppClaims.CompanyId));
        return services;
    }
}
