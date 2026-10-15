using System.Reflection;

namespace AssetSystem.Assets.Api;

/// <summary>Lets the host find this module's endpoint slices without referencing internal types.</summary>
public static class AssetsApiModule
{
    public static Assembly Assembly => typeof(AssetsApiModule).Assembly;
}
