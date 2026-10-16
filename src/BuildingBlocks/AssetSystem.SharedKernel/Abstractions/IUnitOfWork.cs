namespace AssetSystem.SharedKernel.Abstractions;

/// <summary>
/// Commits all changes made through the module's repositories in one transaction and
/// publishes the domain events raised by the changed aggregates via the transactional outbox.
/// </summary>
public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct = default);
}
