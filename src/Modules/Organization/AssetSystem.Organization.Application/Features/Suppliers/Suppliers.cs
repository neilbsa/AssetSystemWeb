using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Contracts;
using AssetSystem.Organization.Domain.Suppliers;

namespace AssetSystem.Organization.Application.Features.Suppliers;

public sealed record SupplierResponse(Guid Id, string Name, string Address, string? ContactNumber, string Email);

public sealed record SearchSuppliersQuery(string? Term);

public static class SearchSuppliersHandler
{
    public static async Task<IReadOnlyList<SupplierResponse>> Handle(
        SearchSuppliersQuery query, ISupplierRepository suppliers, CancellationToken ct) =>
        (await suppliers.SearchAsync(query.Term, 20, ct))
            .Select(s => new SupplierResponse(s.Id, s.Name, s.Address, s.ContactNumber, s.Email))
            .ToList();
}

/// <summary>Reuses a supplier with identical details instead of creating duplicates when assets are registered.</summary>
public static class EnsureSupplierHandler
{
    public static async Task<Guid> Handle(
        EnsureSupplier command, ISupplierRepository suppliers, IOrganizationUnitOfWork uow, CancellationToken ct)
    {
        var existing = await suppliers.FindExactAsync(
            command.Name.Trim(), command.Address.Trim(), command.ContactNumber?.Trim(), command.Email.Trim(), ct);
        if (existing is not null) return existing.Id;

        var supplier = Supplier.Create(command.Name, command.Address, command.ContactNumber, command.Email);
        suppliers.Add(supplier);
        await uow.CommitAsync(ct);
        return supplier.Id;
    }
}
