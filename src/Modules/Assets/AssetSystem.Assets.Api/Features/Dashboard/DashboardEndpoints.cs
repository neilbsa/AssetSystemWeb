using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.Queries;

namespace AssetSystem.Assets.Api.Features.Dashboard;

internal sealed class DashboardEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets").WithTags("Assets · Dashboard").RequireAuthorization(Policies.CompanySelected);

        group.MapGet("/dashboard", async (ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<DashboardResponse>(new GetDashboardQuery(user.RequireCompany()), ct)));

        // Type-ahead for forms: previously used vendors, descriptions and part numbers.
        group.MapGet("/suggestions", async (SuggestionField field, string? q, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<string>>(new SuggestQuery(user.RequireCompany(), field, q), ct)));
    }
}
