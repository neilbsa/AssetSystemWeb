using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Consignments.Domain.Consignments;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

public sealed class ConsignmentsDbContext(DbContextOptions<ConsignmentsDbContext> options) : DbContext(options)
{
    public const string Schema = "consignments";

    public DbSet<Consignment> Consignments => Set<Consignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsignmentsDbContext).Assembly);
        modelBuilder.AddSequences();
        modelBuilder.IgnoreDomainEvents();
    }
}
