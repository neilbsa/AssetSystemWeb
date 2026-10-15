using System.Reflection;

namespace AssetSystem.Consignments.Api;

/// <summary>Lets the host find this module's endpoint slices without referencing internal types.</summary>
public static class ConsignmentsApiModule
{
    public static Assembly Assembly => typeof(ConsignmentsApiModule).Assembly;
}
