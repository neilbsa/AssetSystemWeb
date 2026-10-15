using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.Identity.Infrastructure.Persistence;
using AssetSystem.Identity.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Identity.Infrastructure;

public static class IdentityInfrastructureModule
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddModuleDbContext<IdentityModuleDbContext>(connectionString, IdentityModuleDbContext.Schema);

        services.AddIdentityCore<AppUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityModuleDbContext>();

        services.AddScoped<IUserAccounts, UserAccounts>();
        services.AddSingleton<ITokenIssuer, JwtTokenIssuer>();
        services.AddScoped<IModuleInitializer, IdentityInitializer>();
        return services;
    }
}
