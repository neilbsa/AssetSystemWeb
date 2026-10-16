using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace AssetSystem.BuildingBlocks.Infrastructure.Persistence;

/// <summary>
/// One unit of work per module. Collects the domain events raised by tracked aggregates, enlists
/// them in the Wolverine outbox and saves everything atomically: either the aggregate change and
/// its events are both stored, or neither is.
/// </summary>
public abstract class UnitOfWork<TContext>(IDbContextOutbox<TContext> outbox) : IUnitOfWork
    where TContext : DbContext
{
    public async Task CommitAsync(CancellationToken ct = default)
    {
        var aggregates = outbox.DbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in events)
            await outbox.PublishAsync(domainEvent);

        await outbox.SaveChangesAndFlushMessagesAsync(ct);
    }
}
