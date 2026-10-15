namespace AssetSystem.Assets.Domain.Assets;

/// <summary>
/// Which inventory an asset belongs to. Each category has its own numbering scheme
/// (legacy: "COMP-ASSET" for the Computer Systems Department, "SM-ASSET" for salesman equipment).
/// </summary>
public enum AssetCategory
{
    Company = 0,
    Salesman = 1
}
