using AssetSystem.Assets.Domain.Assets;
using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.UnitTests.Assets;

public class PurchaseTests
{
    private static readonly DateOnly Today = new(2026, 10, 15);

    [Fact]
    public void Depreciates_straight_line_over_the_life_span()
    {
        // 36 months = 1080 days; after 540 days half of the value is gone.
        var purchase = Purchase.Create(54_000m, Today.AddDays(-540), lifeSpanMonths: 36, warrantyMonths: 12, Today);

        Assert.Equal(27_000m, purchase.DepreciatedValue(Today));
    }

    [Fact]
    public void Value_never_drops_below_zero()
    {
        var purchase = Purchase.Create(1_000m, Today.AddYears(-10), lifeSpanMonths: 12, warrantyMonths: 0, Today);

        Assert.Equal(0m, purchase.DepreciatedValue(Today));
    }

    [Fact]
    public void Asset_without_life_span_keeps_its_price()
    {
        var purchase = Purchase.Create(1_000m, Today.AddDays(-30), lifeSpanMonths: 0, warrantyMonths: 0, Today);

        Assert.Equal(1_000m, purchase.DepreciatedValue(Today));
    }

    [Fact]
    public void Warranty_ends_after_the_warranty_months()
    {
        var purchase = Purchase.Create(1m, new DateOnly(2026, 1, 31), 12, warrantyMonths: 12, Today);

        Assert.Equal(new DateOnly(2027, 1, 31), purchase.WarrantyEndsOn);
    }

    [Fact]
    public void Purchase_date_cannot_be_in_the_future()
    {
        var ex = Assert.Throws<DomainException>(() => Purchase.Create(1m, Today.AddDays(1), 12, 12, Today));

        Assert.Equal("Purchase.date_in_future", ex.Code);
    }

    [Fact]
    public void Price_cannot_be_negative()
    {
        Assert.Throws<DomainException>(() => Purchase.Create(-1m, Today, 12, 12, Today));
    }
}
