using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Features.RegisterAsset;
using AssetSystem.Assets.Domain.Assets;

namespace AssetSystem.Assets.Api.Features.RegisterAsset;

internal sealed class RegisterAssetEndpoint : IEndpoint
{
    private sealed record Request(
        AssetCategory Category, Guid BranchId, string Description, string Vendor,
        string? PurchaseOrderNumber, string? ControlNumber, string? Remarks,
        decimal Price, int LifeSpanMonths, int WarrantyMonths,
        string InvoiceNumber, DateOnly InvoiceDate, SupplierInput Supplier, IReadOnlyList<NewItemInput> Items);

    public void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/assets", async (Request body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<RegisteredAssetResponse>>(new RegisterAssetCommand(
                    user.RequireCompany(), body.Category, body.BranchId, body.Description, body.Vendor,
                    body.PurchaseOrderNumber, body.ControlNumber, body.Remarks, body.Price, body.LifeSpanMonths,
                    body.WarrantyMonths, body.InvoiceNumber, body.InvoiceDate, body.Supplier, body.Items), ct))
                .ToCreated(r => $"/api/assets/{r.Id}"))
            .WithTags("Assets")
            .RequireAuthorization(Policies.CompanySelected);
}
