namespace AssetSystem.Consignments.Domain.Consignments;

public enum ConsignmentStatus
{
    /// <summary>Issued and in effect.</summary>
    Open = 0,

    /// <summary>Withdrawn, e.g. because the assets could not be reserved.</summary>
    Cancelled = 1
}

public enum LineStatus
{
    /// <summary>The consignee currently holds this item.</summary>
    Active = 0,

    /// <summary>The item was later issued on another consignment.</summary>
    Superseded = 1,

    Cancelled = 2
}
