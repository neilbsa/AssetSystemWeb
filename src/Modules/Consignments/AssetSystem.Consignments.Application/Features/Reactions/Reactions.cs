using AssetSystem.Assets.Contracts;
using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Contracts;
using AssetSystem.Consignments.Domain.Consignments;
using Microsoft.Extensions.Logging;

namespace AssetSystem.Consignments.Application.Features.Reactions;

/// <summary>
/// Domain event -> integration event. The domain event stays internal to this module; the contract
/// (<see cref="ConsignmentIssued"/>) is what other modules may depend on.
/// </summary>
public static class ConsignmentIssuedDomainEventHandler
{
    public static ConsignmentIssued Handle(ConsignmentIssuedDomainEvent e) =>
        new(e.ConsignmentId, e.CompanyId, e.Number, e.ItemIds);
}

/// <summary>Assets confirmed the reservation: the items' earlier assignments are over.</summary>
public static class AssetItemsReservedHandler
{
    public static async Task Handle(
        AssetItemsReserved reserved, IConsignmentRepository consignments, IConsignmentsUnitOfWork uow, CancellationToken ct)
    {
        var previous = await consignments.FindHoldingItemsAsync(reserved.CompanyId, reserved.ItemIds, reserved.ConsignmentId, ct);
        if (previous.Count == 0) return;

        foreach (var consignment in previous) consignment.SupersedeItems(reserved.ItemIds);
        await uow.CommitAsync(ct);
    }
}

/// <summary>Assets could not reserve the items: the consignment cannot stand.</summary>
public static class AssetItemsReservationRejectedHandler
{
    public static async Task Handle(
        AssetItemsReservationRejected rejected, IConsignmentRepository consignments, IConsignmentsUnitOfWork uow,
        ILogger<AssetItemsReservationRejected> logger, CancellationToken ct)
    {
        var consignment = await consignments.GetAsync(rejected.CompanyId, rejected.ConsignmentId, ct);
        if (consignment is null) return;

        consignment.Cancel(rejected.Reason);
        await uow.CommitAsync(ct);
        logger.LogWarning("Consignment {Number} cancelled: {Reason}", consignment.Number, rejected.Reason);
    }
}
