using System.Reflection;

namespace AssetSystem.Identity.Api;

/// <summary>Lets the host find this module's endpoint slices without referencing internal types.</summary>
public static class IdentityApiModule
{
    public static Assembly Assembly => typeof(IdentityApiModule).Assembly;
}
