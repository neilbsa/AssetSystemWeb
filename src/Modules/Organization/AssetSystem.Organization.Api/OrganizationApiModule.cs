using System.Reflection;

namespace AssetSystem.Organization.Api;

/// <summary>Lets the host find this module's endpoint slices without referencing internal types.</summary>
public static class OrganizationApiModule
{
    public static Assembly Assembly => typeof(OrganizationApiModule).Assembly;
}
