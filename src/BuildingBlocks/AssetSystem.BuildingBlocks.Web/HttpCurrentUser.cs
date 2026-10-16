using System.Security.Claims;
using AssetSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AssetSystem.BuildingBlocks.Web;

/// <summary>Reads the caller from the JWT on the current request; falls back to "system" outside HTTP (background handlers).</summary>
public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public Guid? UserId => Guid.TryParse(Principal?.FindFirstValue("sub"), out var id) ? id : null;

    public string UserName => Principal?.Identity?.IsAuthenticated == true
        ? Principal.Identity.Name ?? "unknown"
        : "system";

    public Guid? CompanyId => Guid.TryParse(Principal?.FindFirstValue(AppClaims.CompanyId), out var id) ? id : null;

    public bool IsAdministrator => Principal?.IsInRole(Roles.Administrator) == true;
}

public static class CurrentUserExtensions
{
    /// <summary>The company the caller selected. Only call behind <see cref="Policies.CompanySelected"/>.</summary>
    public static Guid RequireCompany(this ICurrentUser user) =>
        user.CompanyId ?? throw new InvalidOperationException("No company selected for this request.");
}
