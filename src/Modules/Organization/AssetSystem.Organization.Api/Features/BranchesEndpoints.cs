using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.Branches;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class BranchesEndpoints : IEndpoint
{
    private sealed record BranchRequest(string Name, string Address, string? AssetTag);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/organization/branches").WithTags("Organization · Branches");

        group.MapGet("/", async (ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                Results.Ok(await bus.InvokeAsync<IReadOnlyList<BranchResponse>>(new ListBranchesQuery(user.RequireCompany()), ct)))
            .RequireAuthorization(Policies.CompanySelected);

        group.MapPost("/", async (BranchRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<Guid>>(
                    new CreateBranchCommand(user.RequireCompany(), body.Name, body.Address, body.AssetTag), ct))
                .ToCreated(id => $"/api/organization/branches/{id}"))
            .RequireAuthorization(Policies.CompanyAdministrator);

        group.MapPut("/{id:guid}", async (Guid id, BranchRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result>(
                    new UpdateBranchCommand(user.RequireCompany(), id, body.Name, body.Address, body.AssetTag), ct)).ToHttp())
            .RequireAuthorization(Policies.CompanyAdministrator);
    }
}
