using Microsoft.EntityFrameworkCore;

namespace AssetSystem.BuildingBlocks.Infrastructure.Persistence;

/// <summary>Row of the per-module <c>sequences</c> table.</summary>
public sealed class SequenceCounter
{
    public string Key { get; set; } = default!;
    public long Value { get; set; }
}

public static class SequenceModelExtensions
{
    public const string TableName = "sequences";

    /// <summary>Adds the <c>sequences</c> table to a module's schema.</summary>
    public static void AddSequences(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SequenceCounter>(b =>
        {
            b.ToTable(TableName);
            b.HasKey(x => x.Key);
            b.Property(x => x.Key).HasMaxLength(200);
        });
    }
}

/// <summary>
/// Atomic counter (modules wrap it in a domain-specific numbering service) using a single
/// <c>INSERT ... ON CONFLICT DO UPDATE</c>, so concurrent callers never receive the same number.
/// A number is consumed even if the surrounding use case later fails, so gaps are possible.
/// </summary>
public sealed class SequenceGenerator<TContext>(TContext context, string schema)
    where TContext : DbContext
{
    public async Task<long> NextAsync(string key, CancellationToken ct = default)
    {
        var table = $"\"{schema}\".\"{SequenceModelExtensions.TableName}\"";
        var sql = "INSERT INTO " + table + " (\"Key\", \"Value\") VALUES ({0}, 1) "
                + "ON CONFLICT (\"Key\") DO UPDATE SET \"Value\" = " + table + ".\"Value\" + 1 "
                + "RETURNING \"Value\" AS \"Value\"";

        var rows = await context.Database.SqlQueryRaw<long>(sql, key).ToListAsync(ct);
        return rows.Single();
    }
}
