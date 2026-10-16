using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using AssetSystem.Organization.Domain.ItemTypes;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Organization.Application.Features.ItemTypes;

public sealed record ItemTypeResponse(string Code, string Name);

public sealed record ListItemTypesQuery;

public static class ListItemTypesHandler
{
    public static async Task<IReadOnlyList<ItemTypeResponse>> Handle(
        ListItemTypesQuery _, IItemTypeRepository itemTypes, CancellationToken ct) =>
        (await itemTypes.ListActiveAsync(ct)).Select(t => new ItemTypeResponse(t.Code, t.Name)).ToList();
}

public sealed record CreateItemTypeCommand(string Code, string Name);

public sealed class CreateItemTypeValidator : AbstractValidator<CreateItemTypeCommand>
{
    public CreateItemTypeValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20).Matches("^[A-Za-z0-9]+$");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public static class CreateItemTypeHandler
{
    public static async Task<Result<string>> Handle(
        CreateItemTypeCommand command, IItemTypeRepository itemTypes, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        if (await itemTypes.CodeExistsAsync(command.Code.Trim().ToUpperInvariant(), ct))
            return Error.Conflict("item_type.code_taken", $"Item type code '{command.Code}' already exists.");

        var itemType = ItemType.Create(command.Code, command.Name);
        itemTypes.Add(itemType);
        await uow.CommitAsync(ct);
        return itemType.Code;
    }
}

// ---- Cross-module query -------------------------------------------------------------------

public static class GetActiveItemTypesHandler
{
    public static async Task<IReadOnlyList<ItemTypeDto>> Handle(
        GetActiveItemTypes _, IItemTypeRepository itemTypes, CancellationToken ct) =>
        (await itemTypes.ListActiveAsync(ct)).Select(t => new ItemTypeDto(t.Code, t.Name)).ToList();
}
