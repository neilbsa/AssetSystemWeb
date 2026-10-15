namespace AssetSystem.Assets.Domain.Assets;

public enum ItemStatus
{
    /// <summary>In stock and can be issued to someone.</summary>
    Available = 0,

    /// <summary>Issued through a consignment (legacy name: "OnUsed").</summary>
    InUse = 1,

    Defective = 2
}
