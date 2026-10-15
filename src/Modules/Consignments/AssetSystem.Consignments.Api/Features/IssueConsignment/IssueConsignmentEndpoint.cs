using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Consignments.Application.Features.IssueConsignment;

namespace AssetSystem.Consignments.Api.Features.IssueConsignment;

internal sealed class IssueConsignmentEndpoint : IEndpoint
{
    private sealed record Request(Guid EmployeeId, DateOnly IssuedOn, string? Remarks, IReadOnlyList<Guid> ItemIds);

    public void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/consignments", async (Request body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<IssuedConsignmentResponse>>(
                    new IssueConsignmentCommand(user.RequireCompany(), body.EmployeeId, body.IssuedOn, body.Remarks, body.ItemIds), ct))
                .ToCreated(r => $"/api/consignments/{r.Id}"))
            .WithTags("Consignments")
            .RequireAuthorization(Policies.CompanySelected);
}
