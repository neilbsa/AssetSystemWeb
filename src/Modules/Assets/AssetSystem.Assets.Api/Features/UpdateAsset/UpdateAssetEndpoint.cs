using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Features.RegisterAsset;
using AssetSystem.Assets.Application.Features.UpdateAsset;

namespace AssetSystem.Assets.Api.Features.UpdateAsset;

internal sealed class UpdateAssetEndpoint : IEndpoint
{
    private sealed record Request(
        string Description, string Vendor, string? PurchaseOrderNumber, string? ControlNumber, string? Remarks,
        decimal Price, int LifeSpanMonths, int WarrantyMonths,
        string InvoiceNumber, DateOnly InvoiceDate, SupplierInput Supplier);

    public void Map(IEndpointRouteBuilder app) =>
        app.MapPut("/api/assets/{id:guid}", async (Guid id, Request body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result>(new UpdateAssetCommand(
                    user.RequireCompany(), id, body.Description, body.Vendor, body.PurchaseOrderNumber, body.ControlNumber,
                    body.Remarks, body.Price, body.LifeSpanMonths, body.WarrantyMonths, body.InvoiceNumber,
                    body.InvoiceDate, body.Supplier), ct)).ToHttp())
            .WithTags("Assets")
            .RequireAuthorization(Policies.CompanySelected);
}
