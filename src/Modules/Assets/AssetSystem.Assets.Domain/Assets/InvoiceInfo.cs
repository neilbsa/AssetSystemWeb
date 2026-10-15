using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Assets.Domain.Assets;

/// <summary>Supplier invoice the asset was bought with. Supplier name is a snapshot; the supplier record lives in Organization.</summary>
public sealed class InvoiceInfo : ValueObject
{
    private InvoiceInfo() { }

    private InvoiceInfo(string number, DateOnly date, Guid supplierId, string supplierName)
    {
        Number = number;
        Date = date;
        SupplierId = supplierId;
        SupplierName = supplierName;
    }

    public string Number { get; private set; } = default!;
    public DateOnly Date { get; private set; }
    public Guid SupplierId { get; private set; }
    public string SupplierName { get; private set; } = default!;

    public static InvoiceInfo Create(string number, DateOnly date, Guid supplierId, string supplierName, DateOnly today)
    {
        Guard.That(date <= today, "Invoice.date_in_future", "Invoice date cannot be in the future.");
        return new InvoiceInfo(Guard.NotBlank(number, nameof(Number), 100), date, supplierId,
            Guard.NotBlank(supplierName, nameof(SupplierName)));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Number;
        yield return Date;
        yield return SupplierId;
    }
}
