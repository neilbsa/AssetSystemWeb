using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.RegisterAsset;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Organization.Contracts;
using AssetSystem.SharedKernel.Results;
using FluentValidation;
using Wolverine;

namespace AssetSystem.Assets.Application.Features.Items;

// ---- Add items to an existing asset (an "upgrade") ----------------------------------------------

public sealed record AddItemsCommand(Guid CompanyId, Guid AssetId, IReadOnlyList<NewItemInput> Items);

public sealed class AddItemsValidator : AbstractValidator<AddItemsCommand>
{
    public AddItemsValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.TypeCode).NotEmpty();
            item.RuleFor(i => i.Description).NotEmpty().MaximumLength(200);
            item.RuleFor(i => i.PartNumber).NotEmpty().MaximumLength(100);
        });
    }
}

public static class AddItemsHandler
{
    public static async Task<Result<IReadOnlyList<string>>> Handle(
        AddItemsCommand command, IAssetRepository assets, IAssetQueries queries, IAssetNumbering numbering,
        IMessageBus bus, IAssetsUnitOfWork uow, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");

        var company = await bus.InvokeAsync<CompanyDto?>(new GetCompany(command.CompanyId), ct);
        var branch = await bus.InvokeAsync<BranchDto?>(new GetBranch(command.CompanyId, asset.BranchId), ct);
        if (company is null || branch is null) return Error.Validation("branch.not_found", "The asset's branch no longer exists.");

        var types = (await bus.InvokeAsync<IReadOnlyList<ItemTypeDto>>(new GetActiveItemTypes(), ct))
            .ToDictionary(t => t.Code, StringComparer.OrdinalIgnoreCase);
        var unknown = command.Items.FirstOrDefault(i => !types.ContainsKey(i.TypeCode));
        if (unknown is not null) return Error.Validation("item_type.unknown", $"Unknown item type '{unknown.TypeCode}'.");

        var conflict = await ItemIdentifiers.CheckAsync(
            queries, command.CompanyId, command.Items.Select(i => (i.SerialNumber, i.KeyDetail)), null, ct);
        if (conflict is not null) return conflict;

        var codes = new List<string>();
        foreach (var input in command.Items)
        {
            var type = types[input.TypeCode];
            var code = await numbering.NextItemCodeAsync(asset.Category, command.CompanyId, company.AssetTag, branch.AssetTag, type.Code, ct);
            asset.AddItem(code, type.Code, type.Name, input.Description, input.PartNumber,
                input.SerialNumber, input.KeyDetail, input.ControlNumber, input.Remarks);
            codes.Add(code);
        }

        await uow.CommitAsync(ct);
        return Result.Success<IReadOnlyList<string>>(codes);
    }
}

// ---- Edit an item's descriptive fields ----------------------------------------------------------

public sealed record UpdateItemCommand(
    Guid CompanyId, Guid AssetId, Guid ItemId, string Description, string PartNumber,
    string SerialNumber, string KeyDetail, string? ControlNumber, string? Remarks);

public sealed class UpdateItemValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PartNumber).NotEmpty().MaximumLength(100);
    }
}

public static class UpdateItemHandler
{
    public static async Task<Result> Handle(
        UpdateItemCommand command, IAssetRepository assets, IAssetQueries queries, IAssetsUnitOfWork uow, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");
        if (asset.FindItem(command.ItemId) is null) return Error.NotFound("item.not_found", "Item not found.");

        var conflict = await ItemIdentifiers.CheckAsync(
            queries, command.CompanyId, [(command.SerialNumber, command.KeyDetail)], command.ItemId, ct);
        if (conflict is not null) return conflict;

        asset.UpdateItem(command.ItemId, command.Description, command.PartNumber, command.SerialNumber,
            command.KeyDetail, command.ControlNumber, command.Remarks);
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}

// ---- Mark an item defective / available -----------------------------------------------------------

public sealed record ChangeItemStatusCommand(Guid CompanyId, Guid AssetId, Guid ItemId, ItemStatus Status);

public static class ChangeItemStatusHandler
{
    public static async Task<Result> Handle(
        ChangeItemStatusCommand command, IAssetRepository assets, IAssetsUnitOfWork uow, CancellationToken ct)
    {
        var asset = await assets.GetAsync(command.CompanyId, command.AssetId, ct);
        if (asset is null) return Error.NotFound("asset.not_found", "Asset not found.");
        if (asset.FindItem(command.ItemId) is null) return Error.NotFound("item.not_found", "Item not found.");

        asset.ChangeItemStatus(command.ItemId, command.Status); // DomainException -> 422 when the transition is not allowed
        await uow.CommitAsync(ct);
        return Result.Success();
    }
}
