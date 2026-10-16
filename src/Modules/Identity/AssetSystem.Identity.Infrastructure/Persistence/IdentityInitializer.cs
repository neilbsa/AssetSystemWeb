using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AssetSystem.Identity.Infrastructure.Persistence;

internal sealed class IdentityInitializer(IConfiguration configuration, ILogger<IdentityInitializer> logger)
    : MigratingInitializer<IdentityModuleDbContext>
{
    protected override async Task SeedAsync(IdentityModuleDbContext db, IServiceProvider services, CancellationToken ct)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { Roles.Administrator, Roles.Common })
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole<Guid>(role) { Id = Guid.CreateVersion7() });

        // First start only: create the initial administrator from configuration.
        var password = configuration["Seeding:AdminPassword"];
        if (await db.Users.AnyAsync(ct) || string.IsNullOrWhiteSpace(password)) return;

        var users = services.GetRequiredService<UserManager<AppUser>>();
        var admin = new AppUser
        {
            Id = Guid.CreateVersion7(),
            UserName = configuration["Seeding:AdminUserName"] ?? "administrator",
            Email = configuration["Seeding:AdminEmail"] ?? "administrator@example.com",
            FirstName = "System",
            LastName = "Administrator",
            LockoutEnabled = true
        };

        var created = await users.CreateAsync(admin, password);
        if (!created.Succeeded)
        {
            logger.LogError("Could not create the initial administrator: {Errors}",
                string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        await users.AddToRoleAsync(admin, Roles.Administrator);
        logger.LogWarning("Created initial administrator '{UserName}'. Change its password after first sign-in.", admin.UserName);
    }
}
