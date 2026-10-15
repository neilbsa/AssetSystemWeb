using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Application.Features.Reservations;
using AssetSystem.Assets.Contracts;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Consignments.Contracts;
using Microsoft.Extensions.Logging.Abstractions;

namespace AssetSystem.UnitTests.Assets;

/// <summary>The cross-module reaction: Consignments issued something, Assets decides whether it can be honoured.</summary>
public class ConsignmentIssuedHandlerTests
{
    private static readonly DateOnly Today = new(2026, 10, 15);
    private readonly Guid _company = Guid.NewGuid();

    private Asset AssetWithItems(int count)
    {
        var asset = Asset.Register(_company, AssetCategory.Company, "2CS0001", Guid.NewGuid(), "Desktop", "Dell", null, null, null,
            Purchase.Create(1m, Today, 12, 12, Today), InvoiceInfo.Create("I", Today, Guid.NewGuid(), "S", Today));
        for (var i = 0; i < count; i++)
            asset.AddItem($"CODE-{i}", "M", "Monitor", "Dell", "PN", $"SN{i}", "N/A", null, null);
        return asset;
    }

    private ConsignmentIssued Issued(IEnumerable<Guid> itemIds) =>
        new(Guid.NewGuid(), _company, "00001", itemIds.ToList());

    [Fact]
    public async Task Reserves_available_items_and_confirms()
    {
        var asset = AssetWithItems(2);
        var repo = new FakeRepository(asset);
        var uow = new FakeUnitOfWork();
        var issued = Issued(asset.Items.Select(i => i.Id));

        var outgoing = await ConsignmentIssuedHandler.Handle(issued, repo, uow, NullLogger<ConsignmentIssued>.Instance, TestContext.Current.CancellationToken);

        Assert.All(asset.Items, i => Assert.Equal(ItemStatus.InUse, i.Status));
        Assert.Equal(1, uow.Commits);
        var reserved = Assert.IsType<AssetItemsReserved>(Assert.Single(outgoing));
        Assert.Equal(issued.ConsignmentId, reserved.ConsignmentId);
    }

    [Fact]
    public async Task Rejects_everything_when_one_item_is_no_longer_available()
    {
        var asset = AssetWithItems(2);
        var taken = asset.Items.First();
        asset.IssueItems([taken.Id]); // someone else got there first
        var repo = new FakeRepository(asset);
        var uow = new FakeUnitOfWork();
        var issued = Issued(asset.Items.Select(i => i.Id));

        var outgoing = await ConsignmentIssuedHandler.Handle(issued, repo, uow, NullLogger<ConsignmentIssued>.Instance, TestContext.Current.CancellationToken);

        var rejected = Assert.IsType<AssetItemsReservationRejected>(Assert.Single(outgoing));
        Assert.Contains(taken.ItemCode, rejected.Reason);
        Assert.Equal(0, uow.Commits);
        // all-or-nothing: the still-available item stays available
        Assert.Equal(ItemStatus.Available, asset.Items.Last().Status);
    }

    [Fact]
    public async Task Rejects_when_an_item_does_not_exist()
    {
        var asset = AssetWithItems(1);
        var repo = new FakeRepository(asset);
        var issued = Issued([asset.Items.Single().Id, Guid.NewGuid()]);

        var outgoing = await ConsignmentIssuedHandler.Handle(issued, repo, new FakeUnitOfWork(), NullLogger<ConsignmentIssued>.Instance, TestContext.Current.CancellationToken);

        Assert.IsType<AssetItemsReservationRejected>(Assert.Single(outgoing));
    }

    private sealed class FakeRepository(params Asset[] assets) : IAssetRepository
    {
        public void Add(Asset asset) => throw new NotSupportedException();
        public Task<Asset?> GetAsync(Guid companyId, Guid assetId, CancellationToken ct) => throw new NotSupportedException();

        public Task<IReadOnlyList<Asset>> GetByItemIdsAsync(Guid companyId, IReadOnlyCollection<Guid> itemIds, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Asset>>(assets.Where(a => a.Items.Any(i => itemIds.Contains(i.Id))).ToList());
    }

    private sealed class FakeUnitOfWork : IAssetsUnitOfWork
    {
        public int Commits { get; private set; }

        public Task CommitAsync(CancellationToken ct = default)
        {
            Commits++;
            return Task.CompletedTask;
        }
    }
}
