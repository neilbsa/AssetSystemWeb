namespace AssetSystem.Assets.Domain.Assets;

/// <summary>Business rules for how asset numbers and item codes look. Counters live in infrastructure.</summary>
public static class AssetCodes
{
    /// <summary>"2CS0001" for company assets, "2SM0001" for salesman assets.</summary>
    public static string AssetNumber(AssetCategory category, long sequence) =>
        category == AssetCategory.Salesman ? $"2SM{sequence:D4}" : $"2CS{sequence:D4}";

    public static string AssetNumberPrefix(AssetCategory category) =>
        category == AssetCategory.Salesman ? "2SM" : "2CS";

    /// <summary>"CVCDVOM-001" (company tag + branch tag + type code) or "SMM-0001" for salesman assets.</summary>
    public static string ItemCode(AssetCategory category, string companyTag, string branchTag, string typeCode, long sequence) =>
        category == AssetCategory.Salesman
            ? $"SM{typeCode}-{sequence:D4}"
            : $"{companyTag}{branchTag}{typeCode}-{sequence:D3}";

    /// <summary>The counter an item code is drawn from. Salesman codes carry no company/branch, so their counter is global per type.</summary>
    public static string ItemSequenceKey(AssetCategory category, Guid companyId, string branchTag, string typeCode) =>
        category == AssetCategory.Salesman
            ? $"item:salesman:{typeCode}"
            : $"item:company:{companyId:N}:{branchTag}:{typeCode}";

    public static string AssetSequenceKey(AssetCategory category) => $"asset-number:{category}".ToLowerInvariant();
}
