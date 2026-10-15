using AssetSystem.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.BuildingBlocks.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>Keeps the in-memory domain event list out of the database model. Call last in OnModelCreating.</summary>
    public static void IgnoreDomainEvents(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(IHasDomainEvents).IsAssignableFrom(t.ClrType)).ToList())
        {
            modelBuilder.Entity(entityType.ClrType).Ignore(nameof(IHasDomainEvents.DomainEvents));
        }
    }
}
