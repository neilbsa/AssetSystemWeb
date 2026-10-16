namespace AssetSystem.SharedKernel.Domain;

/// <summary>
/// Something meaningful that happened inside an aggregate. Domain events are raised by
/// aggregates and published (through the transactional outbox) when the aggregate is saved.
/// </summary>
public interface IDomainEvent;
