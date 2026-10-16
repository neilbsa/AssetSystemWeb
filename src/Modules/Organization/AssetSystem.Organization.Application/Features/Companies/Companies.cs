using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using AssetSystem.Organization.Domain.Companies;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Organization.Application.Features.Companies;

public sealed record CompanyResponse(Guid Id, string Code, string Name, string Address, string AssetTag, bool IsActive)
{
    public static CompanyResponse From(Company c) => new(c.Id, c.Code, c.Name, c.Address, c.AssetTag, c.IsActive);
}

// ---- List ---------------------------------------------------------------------------------

public sealed record ListCompaniesQuery;

public static class ListCompaniesHandler
{
    public static async Task<IReadOnlyList<CompanyResponse>> Handle(
        ListCompaniesQuery _, ICompanyRepository companies, CancellationToken ct) =>
        (await companies.ListAsync(ct)).Select(CompanyResponse.From).ToList();
}

// ---- Create -------------------------------------------------------------------------------

public sealed record CreateCompanyCommand(string Code, string Name, string Address, string AssetTag);

public sealed class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetTag).NotEmpty().MaximumLength(10);
    }
}

public static class CreateCompanyHandler
{
    public static async Task<Result<Guid>> Handle(
        CreateCompanyCommand command, ICompanyRepository companies, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        if (await companies.CodeExistsAsync(command.Code.Trim(), ct))
            return Error.Conflict("company.code_taken", $"Company code '{command.Code}' already exists.");

        var company = Company.Create(command.Code, command.Name, command.Address, command.AssetTag);
        companies.Add(company);
        await uow.CommitAsync(ct);
        return company.Id;
    }
}

// ---- Update -------------------------------------------------------------------------------

public sealed record UpdateCompanyCommand(Guid Id, string Name, string Address, string AssetTag);

public sealed class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AssetTag).NotEmpty().MaximumLength(10);
    }
}

public static class UpdateCompanyHandler
{
    public static async Task<Result> Handle(
        UpdateCompanyCommand command, ICompanyRepository companies, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var company = await companies.GetAsync(command.Id, ct);
        if (company is null) return Error.NotFound("company.not_found", "Company not found.");

        company.Update(command.Name, command.Address, command.AssetTag);
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}

// ---- Cross-module queries (see Organization.Contracts) -------------------------------------

public static class GetCompaniesByIdsHandler
{
    public static async Task<IReadOnlyList<CompanyDto>> Handle(
        GetCompaniesByIds query, ICompanyRepository companies, CancellationToken ct) =>
        (await companies.GetByIdsAsync(query.Ids, ct))
            .Where(c => c.IsActive)
            .Select(c => new CompanyDto(c.Id, c.Code, c.Name, c.AssetTag))
            .ToList();
}

public static class GetCompanyHandler
{
    public static async Task<CompanyDto?> Handle(GetCompany query, ICompanyRepository companies, CancellationToken ct)
    {
        var c = await companies.GetAsync(query.CompanyId, ct);
        return c is null ? null : new CompanyDto(c.Id, c.Code, c.Name, c.AssetTag);
    }
}

public static class GetActiveCompaniesHandler
{
    public static async Task<IReadOnlyList<CompanyDto>> Handle(
        GetActiveCompanies _, ICompanyRepository companies, CancellationToken ct) =>
        (await companies.ListAsync(ct))
            .Where(c => c.IsActive)
            .Select(c => new CompanyDto(c.Id, c.Code, c.Name, c.AssetTag))
            .ToList();
}
