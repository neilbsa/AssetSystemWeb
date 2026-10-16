using AssetSystem.BuildingBlocks.Web;
using AssetSystem.Organization.Application.Features.Employees;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace AssetSystem.Organization.Api.Features;

internal sealed class EmployeesEndpoints : IEndpoint
{
    private sealed record RegisterRequest(
        string EmployeeNumber, string FirstName, string? MiddleName, string LastName,
        string Email, Guid DepartmentId, Guid BranchId);

    private sealed record UpdateRequest(
        string FirstName, string? MiddleName, string LastName, string Email, Guid DepartmentId, Guid BranchId);

    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/organization/employees")
            .WithTags("Organization · Employees")
            .RequireAuthorization(Policies.CompanySelected);

        group.MapGet("/", async (string? search, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            Results.Ok(await bus.InvokeAsync<IReadOnlyList<EmployeeResponse>>(new ListEmployeesQuery(user.RequireCompany(), search), ct)));

        group.MapGet("/{id:guid}", async (Guid id, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<EmployeeResponse>>(new GetEmployeeQuery(user.RequireCompany(), id), ct)).ToHttp());

        group.MapGet("/by-number/{employeeNumber}", async (string employeeNumber, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result<EmployeeResponse>>(new FindEmployeeByNumberQuery(user.RequireCompany(), employeeNumber), ct)).ToHttp());

        group.MapPost("/", async (RegisterRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result<Guid>>(new RegisterEmployeeCommand(
                    user.RequireCompany(), body.EmployeeNumber, body.FirstName, body.MiddleName, body.LastName,
                    body.Email, body.DepartmentId, body.BranchId), ct))
                .ToCreated(id => $"/api/organization/employees/{id}"))
            .RequireAuthorization(Policies.CompanyAdministrator);

        group.MapPut("/{id:guid}", async (Guid id, UpdateRequest body, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result>(new UpdateEmployeeCommand(
                    user.RequireCompany(), id, body.FirstName, body.MiddleName, body.LastName,
                    body.Email, body.DepartmentId, body.BranchId), ct)).ToHttp())
            .RequireAuthorization(Policies.CompanyAdministrator);

        group.MapDelete("/{id:guid}", async (Guid id, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
                (await bus.InvokeAsync<Result>(new DeactivateEmployeeCommand(user.RequireCompany(), id), ct)).ToHttp())
            .RequireAuthorization(Policies.CompanyAdministrator);
    }
}
