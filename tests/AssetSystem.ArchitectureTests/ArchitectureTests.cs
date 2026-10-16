using System.Reflection;
using NetArchTest.Rules;

namespace AssetSystem.ArchitectureTests;

/// <summary>
/// The rules that keep the modular monolith modular and the layers clean. When one of these fails,
/// the fix is almost never in the test: it is a dependency pointing the wrong way.
/// </summary>
public class ArchitectureTests
{
    private static readonly string[] Modules = ["Identity", "Organization", "Assets", "Consignments"];

    private static Assembly Load(string module, string layer) =>
        Assembly.Load($"AssetSystem.{module}.{layer}");

    private static string Ns(string module, string layer) => $"AssetSystem.{module}.{layer}";

    public static TheoryData<string> ModuleNames => new(Modules);

    // ---- Clean architecture: dependencies point inwards ---------------------------------------

    [Theory, MemberData(nameof(ModuleNames))]
    public void Domain_depends_on_nothing_but_the_shared_kernel(string module)
    {
        var result = Types.InAssembly(Load(module, "Domain")).ShouldNot()
            .HaveDependencyOnAny(
                Ns(module, "Application"), Ns(module, "Infrastructure"), Ns(module, "Api"),
                "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Wolverine", "FluentValidation", "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Theory, MemberData(nameof(ModuleNames))]
    public void Application_does_not_know_about_infrastructure_or_the_web(string module)
    {
        var result = Types.InAssembly(Load(module, "Application")).ShouldNot()
            .HaveDependencyOnAny(
                Ns(module, "Infrastructure"), Ns(module, "Api"),
                "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Theory, MemberData(nameof(ModuleNames))]
    public void Api_never_reaches_into_infrastructure_or_the_database(string module)
    {
        var result = Types.InAssembly(Load(module, "Api")).ShouldNot()
            .HaveDependencyOnAny(Ns(module, "Infrastructure"), "Microsoft.EntityFrameworkCore", "Npgsql")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    // ---- Modular monolith: modules only meet at their contracts ---------------------------------

    [Theory, MemberData(nameof(ModuleNames))]
    public void A_module_only_references_other_modules_through_their_contracts(string module)
    {
        var others = Modules.Where(m => m != module).ToArray();
        var forbidden = others.SelectMany(o => new[]
            { Ns(o, "Domain"), Ns(o, "Application"), Ns(o, "Infrastructure"), Ns(o, "Api") }).ToArray();

        foreach (var layer in new[] { "Domain", "Application", "Infrastructure", "Api" })
        {
            var result = Types.InAssembly(Load(module, layer)).ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
            Assert.True(result.IsSuccessful, $"{module}.{layer}: {Describe(result)}");
        }
    }

    [Theory]
    [InlineData("Organization")]
    [InlineData("Assets")]
    [InlineData("Consignments")]
    public void Contracts_are_plain_messages_with_no_dependencies_on_the_module(string module)
    {
        var result = Types.InAssembly(Load(module, "Contracts")).ShouldNot()
            .HaveDependencyOnAny(
                Ns(module, "Domain"), Ns(module, "Application"), Ns(module, "Infrastructure"), Ns(module, "Api"),
                "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Wolverine")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    // ---- Vertical slices --------------------------------------------------------------------

    [Theory, MemberData(nameof(ModuleNames))]
    public void Every_endpoint_slice_is_discoverable(string module)
    {
        var endpoints = Types.InAssembly(Load(module, "Api"))
            .That().ImplementInterface(typeof(AssetSystem.BuildingBlocks.Web.IEndpoint))
            .GetTypes();

        Assert.NotEmpty(endpoints);
        Assert.All(endpoints, t => Assert.True(t.IsSealed, $"{t.Name} should be sealed"));
    }

    private static string Describe(NetArchTest.Rules.TestResult result) =>
        result.IsSuccessful ? "" : "Offending types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
