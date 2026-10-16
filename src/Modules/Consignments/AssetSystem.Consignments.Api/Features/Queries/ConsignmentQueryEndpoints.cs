using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Application.Features.Queries;

namespace AssetSystem.Consignments.Api.Features.Queries;

internal sealed class ConsignmentQueryEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/consignments").WithTags("Consignments").RequireAuthorization(Policies.CompanySelected);

        group.MapGet("/", async (Guid? consigneeId, string? search, int? page, int? pageSize,
                ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<PagedResult<ConsignmentSummary>>(
                new ListConsignmentsQuery(user.RequireCompany(), consigneeId, search, page, pageSize), ct)));

        group.MapGet("/consignees", async (string? search, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<ConsigneeSummary>>(new ListConsigneesQuery(user.RequireCompany(), search), ct)));

        group.MapGet("/{id:guid}", async (Guid id, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<ConsignmentDetailsResponse>>(new GetConsignmentQuery(user.RequireCompany(), id), ct)).ToHttp());
    }
}
