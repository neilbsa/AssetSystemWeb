using AssetSystem.Consignments.Domain.Consignments;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;

namespace AssetSystem.Consignments.Application.Abstractions;

public interface IConsignmentsUnitOfWork : IUnitOfWork;

/// <summary>Write side: whole <see cref="Consignment"/> aggregates.</summary>
public interface IConsignmentRepository
{
    void Add(Consignment consignment);
    Task<Consignment?> GetAsync(Guid companyId, Guid id, CancellationToken ct);

    /// <summary>Other consignments of the company that still actively hold any of these items.</summary>
    Task<IReadOnlyList<Consignment>> FindHoldingItemsAsync(
        Guid companyId, IReadOnlyCollection<Guid> itemIds, Guid exceptConsignmentId, CancellationToken ct);
}

public interface IConsignmentNumbering
{
    Task<string> NextNumberAsync(Guid companyId, CancellationToken ct);
}

public sealed record ConsignmentSummary(
    Guid Id, string Number, Guid ConsigneeId, string ConsigneeNumber, string ConsigneeName,
    DateOnly IssuedOn, string Status, int ItemCount, bool HasReceivingCopy);

public sealed record ConsigneeSummary(Guid ConsigneeId, string ConsigneeNumber, string ConsigneeName, string? DepartmentName,
    int ConsignmentCount, int ActiveItemCount);

public sealed record ItemHistoryEntry(
    Guid ConsignmentId, string Number, string ConsigneeName, DateOnly IssuedOn, string ConsignmentStatus, string LineStatus);

/// <summary>Read side: projections straight from the database.</summary>
public interface IConsignmentQueries
{
    Task<PagedResult<ConsignmentSummary>> SearchAsync(
        Guid companyId, Guid? consigneeId, string? search, int page, int pageSize, CancellationToken ct);

    Task<IReadOnlyList<ConsigneeSummary>> ListConsigneesAsync(Guid companyId, string? search, CancellationToken ct);

    Task<IReadOnlyList<ItemHistoryEntry>> ItemHistoryAsync(Guid companyId, Guid itemId, CancellationToken ct);
}
