using AssetSystem.Assets.Domain.Assets;

namespace AssetSystem.UnitTests.Assets;

public class AssetCodesTests
{
    [Theory]
    [InlineData(AssetCategory.Company, 1, "2CS0001")]
    [InlineData(AssetCategory.Company, 42, "2CS0042")]
    [InlineData(AssetCategory.Salesman, 7, "2SM0007")]
    public void Asset_numbers_follow_the_category_prefix(AssetCategory category, long sequence, string expected) =>
        Assert.Equal(expected, AssetCodes.AssetNumber(category, sequence));

    [Fact]
    public void Company_item_codes_combine_company_branch_and_type() =>
        Assert.Equal("CVCDVOM-003", AssetCodes.ItemCode(AssetCategory.Company, "CVC", "DVO", "M", 3));

    [Fact]
    public void Head_office_items_have_no_branch_segment() =>
        Assert.Equal("CVCKYB-012", AssetCodes.ItemCode(AssetCategory.Company, "CVC", "", "KYB", 12));

    [Fact]
    public void Salesman_item_codes_ignore_company_and_branch() =>
        Assert.Equal("SMLCO-0005", AssetCodes.ItemCode(AssetCategory.Salesman, "CVC", "DVO", "LCO", 5));

    [Fact]
    public void Salesman_counters_are_global_per_type_so_codes_cannot_collide()
    {
        var a = AssetCodes.ItemSequenceKey(AssetCategory.Salesman, Guid.NewGuid(), "DVO", "LCO");
        var b = AssetCodes.ItemSequenceKey(AssetCategory.Salesman, Guid.NewGuid(), "CEB", "LCO");

        Assert.Equal(a, b);
    }

    [Fact]
    public void Company_counters_are_per_company_branch_and_type()
    {
        var company = Guid.NewGuid();

        Assert.NotEqual(
            AssetCodes.ItemSequenceKey(AssetCategory.Company, company, "DVO", "M"),
            AssetCodes.ItemSequenceKey(AssetCategory.Company, company, "CEB", "M"));
    }
}
