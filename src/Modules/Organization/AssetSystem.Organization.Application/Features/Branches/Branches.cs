using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using AssetSystem.Organization.Domain.Branches;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Organization.Application.Features.Branches;

public sealed record BranchResponse(Guid Id, string Name, string Address, string AssetTag)
{
    public static BranchResponse From(Branch b) => new(b.Id, b.Name, b.Address, b.AssetTag);
}

public sealed record ListBranchesQuery(Guid CompanyId);

public static class ListBranchesHandler
{
    public static async Task<IReadOnlyList<BranchResponse>> Handle(
        ListBranchesQuery query, IBranchRepository branches, CancellationToken ct) =>
        (await branches.ListAsync(query.CompanyId, ct)).Select(BranchResponse.From).ToList();
}

public sealed record CreateBranchCommand(Guid CompanyId, string Name, string Address, string? AssetTag);

public sealed class CreateBranchValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetTag).MaximumLength(10);
    }
}

public static class CreateBranchHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateBranchCommand command, IBranchRepository branches, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var branch = Branch.Create(command.CompanyId, command.Name, command.Address, command.AssetTag);
        branches.Add(branch);
        await uow.CommitAsync(ct);
        return branch.Id;
    }
}

public sealed record UpdateBranchCommand(Guid CompanyId, Guid Id, string Name, string Address, string? AssetTag);

public sealed class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetTag).MaximumLength(10);
    }
}

public static class UpdateBranchHandler
{
    public static async Task<Result> Handle(
        UpdateBranchCommand command, IBranchRepository branches, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var branch = await branches.GetAsync(command.CompanyId, command.Id, ct);
        if (branch is null) return Error.NotFound("branch.not_found", "Branch not found.");

        branch.Update(command.Name, command.Address, command.AssetTag);
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}

// ---- Cross-module query -------------------------------------------------------------------

public static class GetBranchHandler
{
    public static async Task<BranchDto?> Handle(GetBranch query, IBranchRepository branches, CancellationToken ct)
    {
        var b = await branches.GetAsync(query.CompanyId, query.BranchId, ct);
        return b is null ? null : new BranchDto(b.Id, b.CompanyId, b.Name, b.AssetTag);
    }
}
