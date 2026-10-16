using AssetSystem.Consignments.Domain.Consignments;
using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.UnitTests.Consignments;

public class ConsignmentTests
{
    private static readonly DateOnly Today = new(2026, 10, 15);

    private static NewConsignmentLine Line(Guid? itemId = null) =>
        new(itemId ?? Guid.NewGuid(), "CVCM-001", "Monitor", "Dell 24", "SN-1", "2CS0001");

    private static Consignment Issue(params NewConsignmentLine[] lines) =>
        Consignment.Issue(Guid.NewGuid(), "00001", Guid.NewGuid(), "E1", "Neil Abarabar", "IT", Today, null,
            lines.Length == 0 ? [Line()] : lines, Today);

    [Fact]
    public void Issuing_raises_a_domain_event_listing_every_item()
    {
        var first = Line();
        var second = Line();

        var consignment = Issue(first, second);

        var raised = Assert.IsType<ConsignmentIssuedDomainEvent>(Assert.Single(consignment.DomainEvents));
        Assert.Equal([first.ItemId, second.ItemId], raised.ItemIds);
        Assert.Equal(ConsignmentStatus.Open, consignment.Status);
    }

    [Fact]
    public void A_consignment_needs_at_least_one_item() =>
        Assert.Throws<DomainException>(() =>
            Consignment.Issue(Guid.NewGuid(), "1", Guid.NewGuid(), "E1", "N", null, Today, null, [], Today));

    [Fact]
    public void The_same_item_cannot_be_listed_twice()
    {
        var id = Guid.NewGuid();

        var ex = Assert.Throws<DomainException>(() => Issue(Line(id), Line(id)));

        Assert.Equal("Consignment.duplicate_item", ex.Code);
    }

    [Fact]
    public void Issue_date_cannot_be_in_the_future() =>
        Assert.Throws<DomainException>(() =>
            Consignment.Issue(Guid.NewGuid(), "1", Guid.NewGuid(), "E1", "N", null, Today.AddDays(1), null, [Line()], Today));

    [Fact]
    public void Superseding_only_affects_the_given_items()
    {
        var kept = Line();
        var replaced = Line();
        var consignment = Issue(kept, replaced);

        consignment.SupersedeItems([replaced.ItemId]);

        Assert.Equal(LineStatus.Active, consignment.Lines.Single(l => l.ItemId == kept.ItemId).Status);
        Assert.Equal(LineStatus.Superseded, consignment.Lines.Single(l => l.ItemId == replaced.ItemId).Status);
    }

    [Fact]
    public void Cancelling_records_the_reason_and_cancels_every_line()
    {
        var consignment = Issue(Line(), Line());

        consignment.Cancel("Item CVCM-001 is InUse.");

        Assert.Equal(ConsignmentStatus.Cancelled, consignment.Status);
        Assert.Equal("Item CVCM-001 is InUse.", consignment.CancellationReason);
        Assert.All(consignment.Lines, l => Assert.Equal(LineStatus.Cancelled, l.Status));
    }

    [Fact]
    public void Replacing_the_receiving_copy_returns_the_previous_one()
    {
        var consignment = Issue();
        consignment.AttachReceivingCopy("a.pdf", "application/pdf", 1, "key-a");

        var replaced = consignment.AttachReceivingCopy("b.pdf", "application/pdf", 1, "key-b");

        Assert.Equal("key-a", replaced!.StorageKey);
        Assert.Equal("key-b", consignment.ReceivingCopy!.StorageKey);
    }

    [Fact]
    public void Cancelled_consignments_do_not_accept_a_receiving_copy()
    {
        var consignment = Issue();
        consignment.Cancel("x");

        Assert.Throws<DomainException>(() => consignment.AttachReceivingCopy("a.pdf", "application/pdf", 1, "k"));
    }
}
