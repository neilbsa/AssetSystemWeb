using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.BuildingBlocks.Web;

/// <summary>
/// One vertical slice's HTTP surface. Every feature folder contains one class implementing this
/// interface; <see cref="EndpointExtensions.MapEndpoints"/> finds and maps them all.
/// </summary>
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app, Assembly assembly)
    {
        var endpoints = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && t.IsAssignableTo(typeof(IEndpoint)))
            .Select(t => (IEndpoint)Activator.CreateInstance(t)!);

        foreach (var endpoint in endpoints)
            endpoint.Map(app);

        return app;
    }
}
