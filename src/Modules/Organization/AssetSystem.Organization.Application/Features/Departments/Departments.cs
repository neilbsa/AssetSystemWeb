using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Domain.Departments;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Organization.Application.Features.Departments;

public sealed record DepartmentResponse(Guid Id, string Code, string Name)
{
    public static DepartmentResponse From(Department d) => new(d.Id, d.Code, d.Name);
}

public sealed record ListDepartmentsQuery(Guid CompanyId);

public static class ListDepartmentsHandler
{
    public static async Task<IReadOnlyList<DepartmentResponse>> Handle(
        ListDepartmentsQuery query, IDepartmentRepository departments, CancellationToken ct) =>
        (await departments.ListAsync(query.CompanyId, ct)).Select(DepartmentResponse.From).ToList();
}

public sealed record CreateDepartmentCommand(Guid CompanyId, string Code, string Name);

public sealed class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public static class CreateDepartmentHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateDepartmentCommand command, IDepartmentRepository departments, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        if (await departments.CodeExistsAsync(command.CompanyId, command.Code.Trim(), ct))
            return Error.Conflict("department.code_taken", $"Department code '{command.Code}' already exists.");

        var department = Department.Create(command.CompanyId, command.Code, command.Name);
        departments.Add(department);
        await uow.CommitAsync(ct);
        return department.Id;
    }
}
