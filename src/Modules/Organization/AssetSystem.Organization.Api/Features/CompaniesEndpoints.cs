using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.Companies;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class CompaniesEndpoints : IEndpoint
{
    private sealed record CompanyRequest(string Code, string Name, string Address, string AssetTag);
    private sealed record UpdateCompanyRequest(string Name, string Address, string AssetTag);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/organization/companies")
            .WithTags("Organization · Companies")
            .RequireAuthorization(Policies.Administrator);

        group.MapGet("/", async (IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<CompanyResponse>>(new ListCompaniesQuery(), ct)));

        group.MapPost("/", async (CompanyRequest body, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<Guid>>(new CreateCompanyCommand(body.Code, body.Name, body.Address, body.AssetTag), ct))
                .ToCreated(id => $"/api/organization/companies/{id}"));

        group.MapPut("/{id:guid}", async (Guid id, UpdateCompanyRequest body, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result>(new UpdateCompanyCommand(id, body.Name, body.Address, body.AssetTag), ct)).ToHttp());
    }
}
