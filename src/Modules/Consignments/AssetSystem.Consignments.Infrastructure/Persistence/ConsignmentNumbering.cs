using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Consignments.Application.Abstractions;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

public sealed class ConsignmentNumbering(ConsignmentsDbContext db) : IConsignmentNumbering
{
    private readonly SequenceGenerator<ConsignmentsDbContext> _sequences = new(db, ConsignmentsDbContext.Schema);

    public async Task<string> NextNumberAsync(Guid companyId, CancellationToken ct) =>
        (await _sequences.NextAsync($"consignment:{companyId:N}", ct)).ToString("D5");
}
