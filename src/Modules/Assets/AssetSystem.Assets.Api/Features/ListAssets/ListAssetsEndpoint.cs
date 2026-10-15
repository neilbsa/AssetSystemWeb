using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.Queries;
using AssetSystem.Assets.Domain.Assets;

namespace AssetSystem.Assets.Api.Features.ListAssets;

internal sealed class ListAssetsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/assets", async (
                AssetCategory? category, string? search, int? page, int? pageSize,
                ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                Results.Ok(await bus.InvokeAsync<PagedResult<AssetSummary>>(
                    new ListAssetsQuery(user.RequireCompany(), category, search, page, pageSize), ct)))
            .WithTags("Assets")
            .RequireAuthorization(Policies.CompanySelected);
}
