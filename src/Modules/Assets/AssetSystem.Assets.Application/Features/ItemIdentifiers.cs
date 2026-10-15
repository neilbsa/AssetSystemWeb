using AssetSystem.Assets.Application.Abstractions;
using AssetSystem.Assets.Domain.Assets;
using AssetSystem.SharedKernel.Results;

namespace AssetSystem.Assets.Application.Features;

/// <summary>Serial numbers and keys must be unique within a company (except "N/A"). Shared by every slice that creates or edits items.</summary>
internal static class ItemIdentifiers
{
    public static async Task<Error?> CheckAsync(
        IAssetQueries queries, Guid companyId, IEnumerable<(string Serial, string Key)> identifiers,
        Guid? excludeItemId, CancellationToken ct)
    {
        var normalised = identifiers
            .Select(i => (Serial: AssetItem.NormalizeIdentifier(i.Serial), Key: AssetItem.NormalizeIdentifier(i.Key)))
            .ToList();

        var serials = normalised.Select(i => i.Serial).Where(IsReal).ToList();
        var keys = normalised.Select(i => i.Key).Where(IsReal).ToList();

        var duplicate = serials.GroupBy(s => s).FirstOrDefault(g => g.Count() > 1)?.Key;
        if (duplicate is not null)
            return Error.Conflict("item.duplicate_serial", $"Serial number '{duplicate}' is entered more than once.");

        duplicate = keys.GroupBy(k => k).FirstOrDefault(g => g.Count() > 1)?.Key;
        if (duplicate is not null)
            return Error.Conflict("item.duplicate_key", $"Key '{duplicate}' is entered more than once.");

        if (serials.Count == 0 && keys.Count == 0) return null;

        var conflicts = await queries.FindIdentifierConflictsAsync(companyId, serials, keys, excludeItemId, ct);
        if (conflicts.Serials.Count > 0)
            return Error.Conflict("item.serial_in_use", $"Serial number '{conflicts.Serials.First()}' is already registered.");
        if (conflicts.Keys.Count > 0)
            return Error.Conflict("item.key_in_use", $"Key '{conflicts.Keys.First()}' is already registered.");
        return null;
    }

    private static bool IsReal(string value) => value != AssetItem.NotApplicable;
}
