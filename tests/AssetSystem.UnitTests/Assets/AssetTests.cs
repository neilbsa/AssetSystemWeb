using AssetSystem.Assets.Domain.Assets;
using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.UnitTests.Assets;

public class AssetTests
{
    private static readonly DateOnly Today = new(2026, 10, 15);

    private static Asset NewAsset()
    {
        var asset = Asset.Register(
            Guid.NewGuid(), AssetCategory.Company, "2CS0001", Guid.NewGuid(), "Desktop", "Dell", null, null, null,
            Purchase.Create(1000m, Today.AddDays(-10), 36, 12, Today),
            InvoiceInfo.Create("INV-1", Today.AddDays(-10), Guid.NewGuid(), "Supplier", Today));
        asset.AddItem("CVCM-001", "M", "Monitor", "Dell 24", "P2422H", "sn-1", "n/a", null, null);
        return asset;
    }

    [Fact]
    public void New_items_are_available_and_identifiers_are_normalised()
    {
        var item = NewAsset().Items.Single();

        Assert.Equal(ItemStatus.Available, item.Status);
        Assert.Equal("SN-1", item.SerialNumber);
        Assert.Equal(AssetItem.NotApplicable, item.KeyDetail);
    }

    [Fact]
    public void Blank_identifiers_become_not_applicable()
    {
        var asset = NewAsset();
        var item = asset.AddItem("CVCM-002", "M", "Monitor", "Dell 27", "U2723", "  ", "", null, null);

        Assert.Equal("N/A", item.SerialNumber);
        Assert.Equal("N/A", item.KeyDetail);
    }

    [Fact]
    public void Issuing_marks_available_items_in_use()
    {
        var asset = NewAsset();
        var id = asset.Items.Single().Id;

        asset.IssueItems([id]);

        Assert.Equal(ItemStatus.InUse, asset.Items.Single().Status);
    }

    [Fact]
    public void An_item_in_use_cannot_be_issued_again()
    {
        var asset = NewAsset();
        var id = asset.Items.Single().Id;
        asset.IssueItems([id]);

        var ex = Assert.Throws<DomainException>(() => asset.IssueItems([id]));

        Assert.Equal("Item.not_available", ex.Code);
    }

    [Fact]
    public void Defective_items_cannot_be_issued()
    {
        var asset = NewAsset();
        var id = asset.Items.Single().Id;
        asset.ChangeItemStatus(id, ItemStatus.Defective);

        Assert.Throws<DomainException>(() => asset.IssueItems([id]));
    }

    [Fact]
    public void In_use_is_only_reachable_through_a_consignment()
    {
        var asset = NewAsset();

        var ex = Assert.Throws<DomainException>(() => asset.ChangeItemStatus(asset.Items.Single().Id, ItemStatus.InUse));

        Assert.Equal("Item.use_consignment", ex.Code);
    }

    [Fact]
    public void Repaired_items_become_available_again()
    {
        var asset = NewAsset();
        var id = asset.Items.Single().Id;
        asset.ChangeItemStatus(id, ItemStatus.Defective);

        asset.ChangeItemStatus(id, ItemStatus.Available);

        Assert.Equal(ItemStatus.Available, asset.Items.Single().Status);
    }

    [Fact]
    public void Items_of_other_assets_are_rejected()
    {
        var asset = NewAsset();

        Assert.Throws<DomainException>(() => asset.ChangeItemStatus(Guid.NewGuid(), ItemStatus.Defective));
    }

    [Fact]
    public void Removing_a_document_hands_it_back_so_the_file_can_be_deleted()
    {
        var asset = NewAsset();
        var document = asset.AttachDocument("Invoice", "inv.pdf", "application/pdf", 10, "key123");

        var removed = asset.RemoveDocument(document.Id);

        Assert.Equal("key123", removed.StorageKey);
        Assert.Empty(asset.Documents);
    }

    [Fact]
    public void Document_file_names_are_stripped_of_directories()
    {
        var asset = NewAsset();

        var document = asset.AttachDocument("Invoice", "../../etc/passwd", "application/pdf", 10, "key");

        Assert.Equal("passwd", document.FileName);
    }
}
