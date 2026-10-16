using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Organization.Domain.Branches;
using AssetSystem.Organization.Domain.Companies;
using AssetSystem.Organization.Domain.Departments;
using AssetSystem.Organization.Domain.Employees;
using AssetSystem.Organization.Domain.ItemTypes;
using AssetSystem.Organization.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Organization.Infrastructure.Persistence;

public sealed class OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : DbContext(options)
{
    public const string Schema = "organization";

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationDbContext).Assembly);
        modelBuilder.IgnoreDomainEvents();
    }
}
