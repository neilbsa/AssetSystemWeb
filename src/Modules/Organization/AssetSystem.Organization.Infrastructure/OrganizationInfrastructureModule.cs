using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Infrastructure.Persistence;
using AssetSystem.Organization.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AssetSystem.Organization.Infrastructure;

public static class OrganizationInfrastructureModule
{
    public static IServiceCollection AddOrganizationInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddModuleDbContext<OrganizationDbContext>(connectionString, OrganizationDbContext.Schema);

        services.AddScoped<IOrganizationUnitOfWork, OrganizationUnitOfWork>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IItemTypeRepository, ItemTypeRepository>();

        services.AddScoped<IModuleInitializer, OrganizationInitializer>();
        return services;
    }
}
