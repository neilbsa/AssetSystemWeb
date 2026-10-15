using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Assets.Domain.Assets;

/// <summary>How and when an asset was bought; also drives straight-line depreciation.</summary>
public sealed class Purchase : ValueObject
{
    private Purchase() { }

    private Purchase(decimal price, DateOnly date, int lifeSpanMonths, int warrantyMonths)
    {
        Price = price;
        Date = date;
        LifeSpanMonths = lifeSpanMonths;
        WarrantyMonths = warrantyMonths;
    }

    public decimal Price { get; private set; }
    public DateOnly Date { get; private set; }
    public int LifeSpanMonths { get; private set; }
    public int WarrantyMonths { get; private set; }

    public static Purchase Create(decimal price, DateOnly date, int lifeSpanMonths, int warrantyMonths, DateOnly today)
    {
        Guard.That(price >= 0, "Purchase.price_negative", "Price cannot be negative.");
        Guard.That(date <= today, "Purchase.date_in_future", "Purchase date cannot be in the future.");
        Guard.That(lifeSpanMonths >= 0, "Purchase.lifespan_negative", "Life span cannot be negative.");
        Guard.That(warrantyMonths >= 0, "Purchase.warranty_negative", "Warranty cannot be negative.");
        return new Purchase(price, date, lifeSpanMonths, warrantyMonths);
    }

    public DateOnly WarrantyEndsOn => Date.AddMonths(WarrantyMonths);

    /// <summary>
    /// Straight-line depreciation over the life span (a month counts as 30 days), never below zero.
    /// Without a life span the asset is not depreciated.
    /// </summary>
    public decimal DepreciatedValue(DateOnly today)
    {
        if (LifeSpanMonths <= 0) return Price;

        var lifeDays = LifeSpanMonths * 30m;
        var ageDays = Math.Max(0, today.DayNumber - Date.DayNumber);
        var value = Price - ageDays * (Price / lifeDays);
        return Math.Round(Math.Max(0m, value), 2);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Price;
        yield return Date;
        yield return LifeSpanMonths;
        yield return WarrantyMonths;
    }
}
