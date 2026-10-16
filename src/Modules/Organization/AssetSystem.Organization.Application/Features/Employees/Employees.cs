using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using AssetSystem.Organization.Domain.Employees;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Organization.Application.Features.Employees;

public sealed record EmployeeResponse(
    Guid Id, string EmployeeNumber, string FirstName, string? MiddleName, string LastName, string FullName,
    string Email, Guid DepartmentId, string? DepartmentName, Guid BranchId, bool IsActive)
{
    public static EmployeeResponse From(Employee e, string? departmentName) => new(
        e.Id, e.EmployeeNumber, e.FirstName, e.MiddleName, e.LastName, e.FullName,
        e.Email, e.DepartmentId, departmentName, e.BranchId, e.IsActive);
}

// ---- List / search ------------------------------------------------------------------------

public sealed record ListEmployeesQuery(Guid CompanyId, string? Search);

public static class ListEmployeesHandler
{
    public static async Task<IReadOnlyList<EmployeeResponse>> Handle(
        ListEmployeesQuery query, IEmployeeRepository employees, IDepartmentRepository departments, CancellationToken ct)
    {
        var names = (await departments.ListAsync(query.CompanyId, ct)).ToDictionary(d => d.Id, d => d.Name);
        return (await employees.ListAsync(query.CompanyId, query.Search, ct))
            .Select(e => EmployeeResponse.From(e, names.GetValueOrDefault(e.DepartmentId)))
            .ToList();
    }
}

// ---- Get by id / by employee number ---------------------------------------------------------

public sealed record GetEmployeeQuery(Guid CompanyId, Guid Id);

public static class GetEmployeeQueryHandler
{
    public static async Task<Result<EmployeeResponse>> Handle(
        GetEmployeeQuery query, IEmployeeRepository employees, IDepartmentRepository departments, CancellationToken ct)
    {
        var employee = await employees.GetAsync(query.CompanyId, query.Id, ct);
        if (employee is null) return Error.NotFound("employee.not_found", "Employee not found.");

        var department = (await departments.ListAsync(query.CompanyId, ct)).FirstOrDefault(d => d.Id == employee.DepartmentId);
        return EmployeeResponse.From(employee, department?.Name);
    }
}

public sealed record FindEmployeeByNumberQuery(Guid CompanyId, string EmployeeNumber);

public static class FindEmployeeByNumberHandler
{
    public static async Task<Result<EmployeeResponse>> Handle(
        FindEmployeeByNumberQuery query, IEmployeeRepository employees, IDepartmentRepository departments, CancellationToken ct)
    {
        var employee = await employees.GetByNumberAsync(query.CompanyId, query.EmployeeNumber.Trim(), ct);
        if (employee is null || !employee.IsActive) return Error.NotFound("employee.not_found", "Employee not found.");

        var department = (await departments.ListAsync(query.CompanyId, ct)).FirstOrDefault(d => d.Id == employee.DepartmentId);
        return EmployeeResponse.From(employee, department?.Name);
    }
}

// ---- Register -----------------------------------------------------------------------------

public sealed record RegisterEmployeeCommand(
    Guid CompanyId, string EmployeeNumber, string FirstName, string? MiddleName, string LastName,
    string Email, Guid DepartmentId, Guid BranchId);

public sealed class RegisterEmployeeValidator : AbstractValidator<RegisterEmployeeCommand>
{
    public RegisterEmployeeValidator()
    {
        RuleFor(x => x.EmployeeNumber).NotEmpty().MaximumLength(30);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DepartmentId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
    }
}

public static class RegisterEmployeeHandler
{
    public static async Task<Result<Guid>> Handle(
        RegisterEmployeeCommand command, IEmployeeRepository employees, IDepartmentRepository departments,
        IBranchRepository branches, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        if (await employees.GetByNumberAsync(command.CompanyId, command.EmployeeNumber.Trim(), ct) is not null)
            return Error.Conflict("employee.number_taken", $"Employee number '{command.EmployeeNumber}' is already registered.");

        if (await departments.GetAsync(command.CompanyId, command.DepartmentId, ct) is null)
            return Error.Validation("department.not_found", "Department does not exist in this company.");
        if (await branches.GetAsync(command.CompanyId, command.BranchId, ct) is null)
            return Error.Validation("branch.not_found", "Branch does not exist in this company.");

        var employee = Employee.Register(
            command.CompanyId, command.EmployeeNumber, command.FirstName, command.MiddleName,
            command.LastName, command.Email, command.DepartmentId, command.BranchId);

        employees.Add(employee);
        await uow.CommitAsync(ct);
        return employee.Id;
    }
}

// ---- Update / deactivate ------------------------------------------------------------------

public sealed record UpdateEmployeeCommand(
    Guid CompanyId, Guid Id, string FirstName, string? MiddleName, string LastName,
    string Email, Guid DepartmentId, Guid BranchId);

public sealed class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public static class UpdateEmployeeHandler
{
    public static async Task<Result> Handle(
        UpdateEmployeeCommand command, IEmployeeRepository employees, IDepartmentRepository departments,
        IBranchRepository branches, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var employee = await employees.GetAsync(command.CompanyId, command.Id, ct);
        if (employee is null) return Error.NotFound("employee.not_found", "Employee not found.");

        if (await departments.GetAsync(command.CompanyId, command.DepartmentId, ct) is null)
            return Error.Validation("department.not_found", "Department does not exist in this company.");
        if (await branches.GetAsync(command.CompanyId, command.BranchId, ct) is null)
            return Error.Validation("branch.not_found", "Branch does not exist in this company.");

        employee.UpdateDetails(command.FirstName, command.MiddleName, command.LastName,
            command.Email, command.DepartmentId, command.BranchId);
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}

public sealed record DeactivateEmployeeCommand(Guid CompanyId, Guid Id);

public static class DeactivateEmployeeHandler
{
    public static async Task<Result> Handle(
        DeactivateEmployeeCommand command, IEmployeeRepository employees, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var employee = await employees.GetAsync(command.CompanyId, command.Id, ct);
        if (employee is null) return Error.NotFound("employee.not_found", "Employee not found.");

        employee.Deactivate();
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}

// ---- Cross-module query -------------------------------------------------------------------

public static class GetEmployeeContractHandler
{
    public static async Task<EmployeeDto?> Handle(
        GetEmployee query, IEmployeeRepository employees, IDepartmentRepository departments, CancellationToken ct)
    {
        var employee = await employees.GetAsync(query.CompanyId, query.EmployeeId, ct);
        if (employee is null || !employee.IsActive) return null;

        var department = (await departments.ListAsync(query.CompanyId, ct)).FirstOrDefault(d => d.Id == employee.DepartmentId);
        return new EmployeeDto(employee.Id, employee.EmployeeNumber, employee.FullName, employee.Email,
            employee.DepartmentId, department?.Name ?? "");
    }
}
