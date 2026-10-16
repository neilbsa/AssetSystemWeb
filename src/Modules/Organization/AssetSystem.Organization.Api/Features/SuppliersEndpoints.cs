using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.Suppliers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class SuppliersEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/organization/suppliers", async (string? q, IMessageBus bus, CancellationToken ct) =>
                Results.Ok(await bus.InvokeAsync<IReadOnlyList<SupplierResponse>>(new SearchSuppliersQuery(q), ct)))
            .WithTags("Organization · Suppliers")
            .RequireAuthorization(Policies.CompanySelected);
}
