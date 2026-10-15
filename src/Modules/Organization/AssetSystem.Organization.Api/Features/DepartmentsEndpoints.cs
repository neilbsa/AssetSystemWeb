using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.Departments;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class DepartmentsEndpoints : IEndpoint
{
    private sealed record DepartmentRequest(string Code, string Name);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/organization/departments").WithTags("Organization · Departments");

        group.MapGet("/", async (ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                Results.Ok(await bus.InvokeAsync<IReadOnlyList<DepartmentResponse>>(new ListDepartmentsQuery(user.RequireCompany()), ct)))
            .RequireAuthorization(Policies.CompanySelected);

        group.MapPost("/", async (DepartmentRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<Guid>>(new CreateDepartmentCommand(user.RequireCompany(), body.Code, body.Name), ct))
                .ToCreated(id => $"/api/organization/departments/{id}"))
            .RequireAuthorization(Policies.CompanyAdministrator);
    }
}
