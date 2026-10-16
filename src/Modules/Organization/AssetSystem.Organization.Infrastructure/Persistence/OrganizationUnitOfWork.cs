using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Organization.Application.Abstractions;
using Wolverine.EntityFrameworkCore;

namespace AssetSystem.Organization.Infrastructure.Persistence;

public sealed class OrganizationUnitOfWork(IDbContextOutbox<OrganizationDbContext> outbox)
    : UnitOfWork<OrganizationDbContext>(outbox), IOrganizationUnitOfWork;
