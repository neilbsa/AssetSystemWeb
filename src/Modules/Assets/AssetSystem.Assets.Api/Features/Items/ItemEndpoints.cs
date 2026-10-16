using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.Items;
using AssetSystem.Assets.Application.Features.Queries;
using AssetSystem.Assets.Application.Features.RegisterAsset;
using AssetSystem.Assets.Domain.Assets;

namespace AssetSystem.Assets.Api.Features.Items;

/// <summary>Everything about individual items: inventory list, lookup, history and edits.</summary>
internal sealed class ItemEndpoints : IEndpoint
{
    private sealed record AddItemsRequest(IReadOnlyList<NewItemInput> Items);
    private sealed record UpdateItemRequest(
        string Description, string PartNumber, string SerialNumber, string KeyDetail, string? ControlNumber, string? Remarks);
    private sealed record StatusRequest(ItemStatus Status);

    public void Map(IEndpointRouteBuilder app)
    {
        var assets = app.MapGroup("/api/assets").WithTags("Assets · Items").RequireAuthorization(Policies.CompanySelected);

        assets.MapGet("/items", async (
                ItemStatus? status, string? search, int? page, int? pageSize,
                ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<PagedResult<ItemRow>>(
                new ListItemsQuery(user.RequireCompany(), status, search, page, pageSize), ct)));

        assets.MapGet("/items/lookup", async (string code, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<ItemRow>>(new LookupAvailableItemsQuery(user.RequireCompany(), code), ct)));

        assets.MapGet("/items/check", async (
                string? serialNumber, string? keyDetail, Guid? excludeItemId,
                ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IdentifierAvailabilityResponse>(
                new CheckIdentifiersQuery(user.RequireCompany(), serialNumber, keyDetail, excludeItemId), ct)));

        assets.MapGet("/items/{itemId:guid}", async (Guid itemId, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<ItemRow>>(new GetItemQuery(user.RequireCompany(), itemId), ct)).ToHttp());

        assets.MapGet("/items/{itemId:guid}/history", async (Guid itemId, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<ItemHistoryResponse>>(new GetItemHistoryQuery(user.RequireCompany(), itemId), ct)).ToHttp());

        assets.MapPost("/{id:guid}/items", async (Guid id, AddItemsRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<IReadOnlyList<string>>>(new AddItemsCommand(user.RequireCompany(), id, body.Items), ct)).ToHttp());

        assets.MapPut("/{id:guid}/items/{itemId:guid}", async (
                Guid id, Guid itemId, UpdateItemRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result>(new UpdateItemCommand(
                user.RequireCompany(), id, itemId, body.Description, body.PartNumber, body.SerialNumber,
                body.KeyDetail, body.ControlNumber, body.Remarks), ct)).ToHttp());

        assets.MapPut("/{id:guid}/items/{itemId:guid}/status", async (
                Guid id, Guid itemId, StatusRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result>(new ChangeItemStatusCommand(user.RequireCompany(), id, itemId, body.Status), ct)).ToHttp());
    }
}
