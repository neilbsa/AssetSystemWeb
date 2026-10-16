using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Organization.Contracts;
using AssetSystem.SharedKernel.Results;
using FluentValidation;
using Wolverine;

namespace AssetSystem.Assets.Application.Features.RegisterAsset;

public sealed record SupplierInput(string Name, string Address, string? ContactNumber, string Email);

public sealed record NewItemInput(
    string TypeCode, string Description, string PartNumber, string SerialNumber, string KeyDetail,
    string? ControlNumber, string? Remarks);

public sealed record RegisterAssetCommand(
    Guid CompanyId, AssetCategory Category, Guid BranchId, string Description, string Vendor,
    string? PurchaseOrderNumber, string? ControlNumber, string? Remarks,
    decimal Price, int LifeSpanMonths, int WarrantyMonths,
    string InvoiceNumber, DateOnly InvoiceDate, SupplierInput Supplier,
    IReadOnlyList<NewItemInput> Items);

public sealed record RegisteredAssetResponse(Guid Id, string AssetNumber, IReadOnlyList<string> ItemCodes);

public sealed class RegisterAssetValidator : AbstractValidator<RegisterAssetCommand>
{
    public RegisterAssetValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Vendor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LifeSpanMonths).InclusiveBetween(0, 600);
        RuleFor(x => x.WarrantyMonths).InclusiveBetween(0, 600);
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Supplier.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Supplier.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Supplier.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Add at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.TypeCode).NotEmpty();
            item.RuleFor(i => i.Description).NotEmpty().MaximumLength(200);
            item.RuleFor(i => i.PartNumber).NotEmpty().MaximumLength(100);
        });
    }
}

public static class RegisterAssetHandler
{
    public static async Task<Result<RegisteredAssetResponse>> Handle(
        RegisterAssetCommand command, IMessageBus bus, IAssetRepository assets, IAssetQueries queries,
        IAssetNumbering numbering, IAssetsUnitOfWork uow, TimeProvider clock, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);

        var company = await bus.InvokeAsync<CompanyDto?>(new GetCompany(command.CompanyId), ct);
        if (company is null) return Error.NotFound("company.not_found", "Company not found.");

        var branch = await bus.InvokeAsync<BranchDto?>(new GetBranch(command.CompanyId, command.BranchId), ct);
        if (branch is null) return Error.Validation("branch.not_found", "Branch does not exist in this company.");

        var types = (await bus.InvokeAsync<IReadOnlyList<ItemTypeDto>>(new GetActiveItemTypes(), ct))
            .ToDictionary(t => t.Code, StringComparer.OrdinalIgnoreCase);
        var unknown = command.Items.FirstOrDefault(i => !types.ContainsKey(i.TypeCode));
        if (unknown is not null) return Error.Validation("item_type.unknown", $"Unknown item type '{unknown.TypeCode}'.");

        var conflict = await ItemIdentifiers.CheckAsync(
            queries, command.CompanyId, command.Items.Select(i => (i.SerialNumber, i.KeyDetail)), null, ct);
        if (conflict is not null) return conflict;

        var supplier = command.Supplier;
        var supplierId = await bus.InvokeAsync<Guid>(
            new EnsureSupplier(supplier.Name, supplier.Address, supplier.ContactNumber, supplier.Email), ct);

        var asset = Asset.Register(
            command.CompanyId, command.Category, await numbering.NextAssetNumberAsync(command.Category, ct),
            command.BranchId, command.Description, command.Vendor, command.PurchaseOrderNumber,
            command.ControlNumber, command.Remarks,
            Purchase.Create(command.Price, command.InvoiceDate, command.LifeSpanMonths, command.WarrantyMonths, today),
            InvoiceInfo.Create(command.InvoiceNumber, command.InvoiceDate, supplierId, supplier.Name.Trim(), today));

        foreach (var input in command.Items)
        {
            var type = types[input.TypeCode];
            var code = await numbering.NextItemCodeAsync(command.Category, command.CompanyId, company.AssetTag, branch.AssetTag, type.Code, ct);
            asset.AddItem(code, type.Code, type.Name, input.Description, input.PartNumber,
                input.SerialNumber, input.KeyDetail, input.ControlNumber, input.Remarks);
        }

        assets.Add(asset);
        await uow.CommitAsync(ct);

        return new RegisteredAssetResponse(asset.Id, asset.AssetNumber, asset.Items.Select(i => i.ItemCode).ToList());
    }
}
