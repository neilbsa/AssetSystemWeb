using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Features.Queries;

namespace AssetSystem.Assets.Api.Features.GetAsset;

internal sealed class GetAssetEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/assets/{id:guid}", async (Guid id, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<AssetDetailsResponse>>(new GetAssetQuery(user.RequireCompany(), id), ct)).ToHttp())
            .WithTags("Assets")
            .RequireAuthorization(Policies.CompanySelected);
}
