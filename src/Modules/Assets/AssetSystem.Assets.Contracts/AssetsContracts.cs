namespace AssetSystem.Assets.Contracts;

// Public surface of the Assets module (see Organization.Contracts for the idea).

/// <summary>An asset item as seen by the Consignments module when issuing equipment.</summary>
public sealed record ConsignmentItemDto(
    Guid Id, string ItemCode, string TypeName, string Description, string SerialNumber, string AssetNumber, string Status);

/// <summary>Looks up items of one company by id. Unknown ids are simply missing from the answer.</summary>
public sealed record GetItemsForConsignment(Guid CompanyId, IReadOnlyCollection<Guid> ItemIds);

/// <summary>
/// Published when the Assets module could not reserve the items of an issued consignment
/// (for instance because two consignments raced for the same item). Consignments reacts by cancelling it.
/// </summary>
public sealed record AssetItemsReservationRejected(
    Guid ConsignmentId, Guid CompanyId, IReadOnlyList<Guid> ItemIds, string Reason);

/// <summary>Published when every item of a consignment was reserved ("in use"). Consignments then retires the items' older assignments.</summary>
public sealed record AssetItemsReserved(Guid ConsignmentId, Guid CompanyId, IReadOnlyList<Guid> ItemIds);
