namespace AssetSystem.Consignments.Contracts;

// Public surface of the Consignments module.

/// <summary>
/// Integration event: a consignment was issued. Assets reacts by marking the items "in use".
/// Delivered through the transactional outbox, so it is never lost and never sent for a rolled-back consignment.
/// </summary>
public sealed record ConsignmentIssued(
    Guid ConsignmentId, Guid CompanyId, string ConsignmentNumber, IReadOnlyList<Guid> ItemIds);

/// <summary>Every consignment an item has been part of, newest first.</summary>
public sealed record GetItemConsignmentHistory(Guid CompanyId, Guid ItemId);

public sealed record ItemConsignmentDto(
    Guid ConsignmentId, string ConsignmentNumber, string ConsigneeName, DateOnly IssuedOn,
    string ConsignmentStatus, string LineStatus);
