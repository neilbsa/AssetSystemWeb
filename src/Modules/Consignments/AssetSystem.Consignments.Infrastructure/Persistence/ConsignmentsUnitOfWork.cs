using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Consignments.Application.Abstractions;
using Wolverine.EntityFrameworkCore;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

public sealed class ConsignmentsUnitOfWork(IDbContextOutbox<ConsignmentsDbContext> outbox)
    : UnitOfWork<ConsignmentsDbContext>(outbox), IConsignmentsUnitOfWork;

internal sealed class ConsignmentsInitializer : MigratingInitializer<ConsignmentsDbContext>;
