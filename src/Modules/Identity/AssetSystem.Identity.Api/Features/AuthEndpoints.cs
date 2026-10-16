using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Identity.Application.Features;
using AssetSystem.Identity.Application.Features.ChangePassword;
using AssetSystem.Identity.Application.Features.Login;
using AssetSystem.Identity.Application.Features.SelectCompany;
using AssetSystem.Identity.Application.Features.Session;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Identity.Api.Features;

internal sealed class AuthEndpoints : IEndpoint
{
    private sealed record SelectCompanyRequest(Guid CompanyId);
    private sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginCommand body, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<SessionResponse>>(body, ct)).ToHttp())
            .AllowAnonymous()
            .RequireRateLimiting(RateLimiting.Login);

        group.MapPost("/select-company", async (SelectCompanyRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<SessionResponse>>(new SelectCompanyCommand(user.UserId!.Value, body.CompanyId), ct)).ToHttp())
            .RequireAuthorization();

        group.MapGet("/session", async (ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<SessionResponse>>(new GetSessionQuery(user.UserId!.Value, user.CompanyId), ct)).ToHttp())
            .RequireAuthorization();

        group.MapPost("/change-password", async (ChangePasswordRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result>(new ChangePasswordCommand(user.UserId!.Value, body.CurrentPassword, body.NewPassword), ct)).ToHttp())
            .RequireAuthorization();
    }
}
