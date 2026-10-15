using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Consignments.Domain.Consignments;

/// <summary>Raised when a consignment is issued; the Application layer turns it into the public integration event.</summary>
public sealed record ConsignmentIssuedDomainEvent(
    Guid ConsignmentId, Guid CompanyId, string Number, IReadOnlyList<Guid> ItemIds) : IDomainEvent;

/// <summary>
/// A record of equipment handed to one employee (the consignee). Descriptive data about the consignee and
/// the items is copied in at issue time, so the document stays accurate even if the source data changes later.
/// </summary>
public sealed class Consignment : AggregateRoot, IAuditable
{
    private readonly List<ConsignmentLine> _lines = [];

    private Consignment() { }

    public Guid CompanyId { get; private set; }

    /// <summary>Running number per company, e.g. "00042".</summary>
    public string Number { get; private set; } = default!;

    public Guid ConsigneeId { get; private set; }
    public string ConsigneeNumber { get; private set; } = default!;
    public string ConsigneeName { get; private set; } = default!;
    public string? DepartmentName { get; private set; }

    public DateOnly IssuedOn { get; private set; }
    public string? Remarks { get; private set; }
    public ConsignmentStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public ReceivingCopy? ReceivingCopy { get; private set; }

    public IReadOnlyCollection<ConsignmentLine> Lines => _lines;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Consignment Issue(
        Guid companyId, string number, Guid consigneeId, string consigneeNumber, string consigneeName,
        string? departmentName, DateOnly issuedOn, string? remarks, IReadOnlyCollection<NewConsignmentLine> lines, DateOnly today)
    {
        Guard.That(lines.Count > 0, "Consignment.empty", "A consignment needs at least one item.");
        Guard.That(lines.Select(l => l.ItemId).Distinct().Count() == lines.Count,
            "Consignment.duplicate_item", "The same item cannot appear twice on a consignment.");
        Guard.That(issuedOn <= today, "Consignment.date_in_future", "Issue date cannot be in the future.");

        var consignment = new Consignment
        {
            Id = NewId(),
            CompanyId = companyId,
            Number = Guard.NotBlank(number, nameof(Number), 30),
            ConsigneeId = consigneeId,
            ConsigneeNumber = Guard.NotBlank(consigneeNumber, nameof(ConsigneeNumber), 30),
            ConsigneeName = Guard.NotBlank(consigneeName, nameof(ConsigneeName)),
            DepartmentName = Guard.OptionalText(departmentName, nameof(DepartmentName)),
            IssuedOn = issuedOn,
            Remarks = Guard.OptionalText(remarks, nameof(Remarks)),
            Status = ConsignmentStatus.Open
        };

        consignment._lines.AddRange(lines.Select(l =>
            new ConsignmentLine(l.ItemId, l.ItemCode, l.TypeName, l.Description, l.SerialNumber, l.AssetNumber)));

        consignment.Raise(new ConsignmentIssuedDomainEvent(
            consignment.Id, companyId, consignment.Number, consignment._lines.Select(l => l.ItemId).ToList()));
        return consignment;
    }

    /// <summary>Withdraws the consignment because its assets could not be handed out.</summary>
    public void Cancel(string reason)
    {
        if (Status == ConsignmentStatus.Cancelled) return;
        Status = ConsignmentStatus.Cancelled;
        CancellationReason = Guard.NotBlank(reason, nameof(CancellationReason), 500);
        foreach (var line in _lines) line.Cancel();
    }

    /// <summary>Another consignment now holds these items, so this one no longer does.</summary>
    public void SupersedeItems(IReadOnlyCollection<Guid> itemIds)
    {
        foreach (var line in _lines.Where(l => itemIds.Contains(l.ItemId))) line.Supersede();
    }

    /// <summary>Attaches (or replaces) the signed receiving copy; returns the replaced copy so its file can be deleted.</summary>
    public ReceivingCopy? AttachReceivingCopy(string fileName, string contentType, long sizeBytes, string storageKey)
    {
        Guard.That(Status == ConsignmentStatus.Open, "Consignment.not_open", "Only open consignments accept a receiving copy.");
        var previous = ReceivingCopy;
        ReceivingCopy = new ReceivingCopy(fileName, contentType, sizeBytes, storageKey);
        return previous;
    }
}
