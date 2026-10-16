using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.ItemTypes;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class ItemTypesEndpoints : IEndpoint
{
    private sealed record ItemTypeRequest(string Code, string Name);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/organization/item-types").WithTags("Organization · Item types");

        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
                Results.Ok(await bus.InvokeAsync<IReadOnlyList<ItemTypeResponse>>(new ListItemTypesQuery(), ct)))
            .RequireAuthorization(Policies.CompanySelected);

        group.MapPost("/", async (ItemTypeRequest body, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<string>>(new CreateItemTypeCommand(body.Code, body.Name), ct))
                .ToCreated(code => $"/api/organization/item-types/{code}"))
            .RequireAuthorization(Policies.Administrator);
    }
}
