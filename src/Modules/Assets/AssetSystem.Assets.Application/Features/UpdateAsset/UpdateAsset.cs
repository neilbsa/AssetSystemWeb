using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.RegisterAsset;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Organization.Contracts;
using AssetSystem.SharedKernel.Results;
using FluentValidation;
using Wolverine;

namespace AssetSystem.Assets.Application.Features.UpdateAsset;

public sealed record UpdateAssetCommand(
    Guid CompanyId, Guid AssetId, string Description, string Vendor,
    string? PurchaseOrderNumber, string? ControlNumber, string? Remarks,
    decimal Price, int LifeSpanMonths, int WarrantyMonths,
    string InvoiceNumber, DateOnly InvoiceDate, SupplierInput Supplier);

public sealed class UpdateAssetValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Vendor).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LifeSpanMonths).InclusiveBetween(0, 600);
        RuleFor(x => x.WarrantyMonths).InclusiveBetween(0, 600);
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Supplier.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Supplier.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Supplier.Email).NotEmpty().EmailAddress();
    }
}

public static class UpdateAssetHandler
{
    public static async Task<Result> Handle(
        UpdateAssetCommand command, IAssetRepository assets, IMessageBus bus, IAssetsUnitOfWork uow,
        TimeProvider clock, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");

        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);
        var supplier = command.Supplier;
        var supplierId = await bus.InvokeAsync<Guid>(
            new EnsureSupplier(supplier.Name, supplier.Address, supplier.ContactNumber, supplier.Email), ct);

        asset.UpdateDetails(
            command.Description, command.Vendor, command.PurchaseOrderNumber, command.ControlNumber, command.Remarks,
            Purchase.Create(command.Price, command.InvoiceDate, command.LifeSpanMonths, command.WarrantyMonths, today),
            InvoiceInfo.Create(command.InvoiceNumber, command.InvoiceDate, supplierId, supplier.Name.Trim(), today));

        await uow.CommitAsync(ct);
        return Result.Success();
    }
}
