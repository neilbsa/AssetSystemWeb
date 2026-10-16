using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using Wolverine.EntityFrameworkCore;

namespace AssetSystem.Assets.Infrastructure.Persistence;

public sealed class AssetsUnitOfWork(IDbContextOutbox<AssetsDbContext> outbox)
    : UnitOfWork<AssetsDbContext>(outbox), IAssetsUnitOfWork;
