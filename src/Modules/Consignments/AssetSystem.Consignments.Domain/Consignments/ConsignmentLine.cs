using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Consignments.Domain.Consignments;

/// <summary>One asset item on a consignment. Descriptive fields are a snapshot taken at issue time.</summary>
public sealed class ConsignmentLine : Entity
{
    private ConsignmentLine() { }

    internal ConsignmentLine(
        Guid itemId, string itemCode, string typeName, string description, string serialNumber, string assetNumber) : base(NewId())
    {
        ItemId = itemId;
        ItemCode = itemCode;
        TypeName = typeName;
        Description = description;
        SerialNumber = serialNumber;
        AssetNumber = assetNumber;
        Status = LineStatus.Active;
    }

    public Guid ItemId { get; private set; }
    public string ItemCode { get; private set; } = default!;
    public string TypeName { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string SerialNumber { get; private set; } = default!;
    public string AssetNumber { get; private set; } = default!;
    public LineStatus Status { get; private set; }

    internal void Supersede()
    {
        if (Status == LineStatus.Active) Status = LineStatus.Superseded;
    }

    internal void Cancel() => Status = LineStatus.Cancelled;
}

public sealed record NewConsignmentLine(
    Guid ItemId, string ItemCode, string TypeName, string Description, string SerialNumber, string AssetNumber);
