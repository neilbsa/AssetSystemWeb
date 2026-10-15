using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Contracts;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.Consignments.Contracts;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace AssetSystem.Assets.Application.Features.Reservations;

/// <summary>Answers the Consignments module's question "what do these items look like right now?".</summary>
public static class GetItemsForConsignmentHandler
{
    public static async Task<IReadOnlyList<ConsignmentItemDto>> Handle(
        GetItemsForConsignment query, IAssetQueries queries, CancellationToken ct) =>
        (await queries.GetItemsAsync(query.CompanyId, query.ItemIds, ct))
            .Select(i => new ConsignmentItemDto(i.Id, i.ItemCode, i.TypeName, i.Description, i.SerialNumber, i.AssetNumber, i.Status))
            .ToList();
}

/// <summary>
/// Reacts to a consignment being issued: the items it contains become "in use".
/// If any item cannot be reserved (it was taken or marked defective in the meantime) nothing changes
/// and a rejection is published so the consignment gets cancelled. On success a confirmation is published.
/// </summary>
public static class ConsignmentIssuedHandler
{
    public static async Task<OutgoingMessages> Handle(
        ConsignmentIssued issued, IAssetRepository assets, IAssetsUnitOfWork uow,
        ILogger<ConsignmentIssued> logger, CancellationToken ct)
    {
        var owners = await assets.GetByItemIdsAsync(issued.CompanyId, issued.ItemIds, ct);
        var found = owners.SelectMany(a => a.Items).Where(i => issued.ItemIds.Contains(i.Id)).ToList();

        if (found.Count != issued.ItemIds.Count)
            return Reject(issued, "Some items no longer exist.", logger);

        var unavailable = found.FirstOrDefault(i => i.Status != ItemStatus.Available);
        if (unavailable is not null)
            return Reject(issued, $"Item {unavailable.ItemCode} is {unavailable.Status}.", logger);

        foreach (var asset in owners)
            asset.IssueItems(asset.Items.Where(i => issued.ItemIds.Contains(i.Id)).Select(i => i.Id));

        await uow.CommitAsync(ct);
        return [new AssetItemsReserved(issued.ConsignmentId, issued.CompanyId, issued.ItemIds)];
    }

    private static OutgoingMessages Reject(ConsignmentIssued issued, string reason, ILogger logger)
    {
        logger.LogWarning("Rejecting reservation for consignment {Consignment}: {Reason}", issued.ConsignmentNumber, reason);
        return [new AssetItemsReservationRejected(issued.ConsignmentId, issued.CompanyId, issued.ItemIds, reason)];
    }
}
