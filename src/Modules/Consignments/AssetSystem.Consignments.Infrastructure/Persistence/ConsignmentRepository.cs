using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Domain.Consignments;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

public sealed class ConsignmentRepository(ConsignmentsDbContext db) : IConsignmentRepository
{
    public void Add(Consignment consignment) => db.Consignments.Add(consignment);

    public Task<Consignment?> GetAsync(Guid companyId, Guid id, CancellationToken ct) =>
        db.Consignments.Include(c => c.Lines).FirstOrDefaultAsync(c => c.CompanyId == companyId && c.Id == id, ct);

    public async Task<IReadOnlyList<Consignment>> FindHoldingItemsAsync(
        Guid companyId, IReadOnlyCollection<Guid> itemIds, Guid exceptConsignmentId, CancellationToken ct) =>
        await db.Consignments.Include(c => c.Lines)
            .Where(c => c.CompanyId == companyId && c.Id != exceptConsignmentId
                        && c.Lines.Any(l => itemIds.Contains(l.ItemId) && l.Status == LineStatus.Active))
            .ToListAsync(ct);
}
