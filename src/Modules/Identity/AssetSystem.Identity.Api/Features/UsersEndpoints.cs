using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Identity.Application.Features.Users;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Identity.Api.Features;

/// <summary>System accounts (people who can sign in). Administrators only.</summary>
internal sealed class UsersEndpoints : IEndpoint
{
    private sealed record UpdateAccessRequest(
        IReadOnlyList<string> Roles, IReadOnlyList<Guid> CompanyIds, bool IsLocked, string? NewPassword);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/identity/users").WithTags("Identity · Users").RequireAuthorization(Policies.Administrator);

        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<UserResponse>>(new ListUsersQuery(), ct)));

        group.MapGet("/{id:guid}", async (Guid id, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<UserResponse>>(new GetUserQuery(id), ct)).ToHttp());

        group.MapGet("/availability", async (string? userName, string? email, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<AvailabilityResponse>(new CheckAvailabilityQuery(userName, email), ct)));

        group.MapPost("/", async (RegisterUserCommand body, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<Guid>>(body, ct)).ToCreated(id => $"/api/identity/users/{id}"));

        group.MapPut("/{id:guid}/access", async (Guid id, UpdateAccessRequest body, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result>(
                new UpdateUserAccessCommand(id, body.Roles, body.CompanyIds, body.IsLocked, body.NewPassword), ct)).ToHttp());
    }
}
