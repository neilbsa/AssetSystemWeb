using AssetSystem.Organization.Application.Abstractions;
using AssetSystem.Organization.Domain.Branches;
using AssetSystem.Organization.Domain.Companies;
using AssetSystem.Organization.Domain.Departments;
using AssetSystem.Organization.Domain.Employees;
using AssetSystem.Organization.Domain.ItemTypes;
using AssetSystem.Organization.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Organization.Infrastructure.Persistence.Repositories;

public sealed class CompanyRepository(OrganizationDbContext db) : ICompanyRepository
{
    public void Add(Company company) => db.Companies.Add(company);

    public Task<Company?> GetAsync(Guid id, CancellationToken ct) =>
        db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Company>> ListAsync(CancellationToken ct) =>
        await db.Companies.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<Company>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) =>
        await db.Companies.AsNoTracking().Where(c => ids.Contains(c.Id)).OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct) =>
        db.Companies.AnyAsync(c => c.Code == code, ct);
}

public sealed class BranchRepository(OrganizationDbContext db) : IBranchRepository
{
    public void Add(Branch branch) => db.Branches.Add(branch);

    public Task<Branch?> GetAsync(Guid companyId, Guid id, CancellationToken ct) =>
        db.Branches.FirstOrDefaultAsync(b => b.CompanyId == companyId && b.Id == id && b.IsActive, ct);

    public async Task<IReadOnlyList<Branch>> ListAsync(Guid companyId, CancellationToken ct) =>
        await db.Branches.AsNoTracking().Where(b => b.CompanyId == companyId && b.IsActive).OrderBy(b => b.Name).ToListAsync(ct);
}

public sealed class DepartmentRepository(OrganizationDbContext db) : IDepartmentRepository
{
    public void Add(Department department) => db.Departments.Add(department);

    public Task<Department?> GetAsync(Guid companyId, Guid id, CancellationToken ct) =>
        db.Departments.FirstOrDefaultAsync(d => d.CompanyId == companyId && d.Id == id && d.IsActive, ct);

    public async Task<IReadOnlyList<Department>> ListAsync(Guid companyId, CancellationToken ct) =>
        await db.Departments.AsNoTracking().Where(d => d.CompanyId == companyId && d.IsActive).OrderBy(d => d.Code).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(Guid companyId, string code, CancellationToken ct) =>
        db.Departments.AnyAsync(d => d.CompanyId == companyId && d.Code == code, ct);
}

public sealed class SupplierRepository(OrganizationDbContext db) : ISupplierRepository
{
    public void Add(Supplier supplier) => db.Suppliers.Add(supplier);

    public async Task<IReadOnlyList<Supplier>> SearchAsync(string? term, int limit, CancellationToken ct)
    {
        var query = db.Suppliers.AsNoTracking().Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(term))
            query = query.Where(s => EF.Functions.ILike(s.Name, $"%{term.Trim()}%"));
        return await query.OrderBy(s => s.Name).Take(limit).ToListAsync(ct);
    }

    public Task<Supplier?> FindExactAsync(string name, string address, string? contactNumber, string email, CancellationToken ct) =>
        db.Suppliers.FirstOrDefaultAsync(s => s.IsActive && s.Name == name && s.Address == address
                                              && s.ContactNumber == contactNumber && s.Email == email, ct);
}

public sealed class EmployeeRepository(OrganizationDbContext db) : IEmployeeRepository
{
    public void Add(Employee employee) => db.Employees.Add(employee);

    public Task<Employee?> GetAsync(Guid companyId, Guid id, CancellationToken ct) =>
        db.Employees.FirstOrDefaultAsync(e => e.CompanyId == companyId && e.Id == id, ct);

    public Task<Employee?> GetByNumberAsync(Guid companyId, string employeeNumber, CancellationToken ct) =>
        db.Employees.FirstOrDefaultAsync(e => e.CompanyId == companyId && e.EmployeeNumber == employeeNumber, ct);

    public async Task<IReadOnlyList<Employee>> ListAsync(Guid companyId, string? search, CancellationToken ct)
    {
        var query = db.Employees.AsNoTracking().Where(e => e.CompanyId == companyId && e.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(e => EF.Functions.ILike(e.EmployeeNumber, pattern)
                                     || EF.Functions.ILike(e.FirstName, pattern)
                                     || EF.Functions.ILike(e.LastName, pattern));
        }
        return await query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Take(200).ToListAsync(ct);
    }
}

public sealed class ItemTypeRepository(OrganizationDbContext db) : IItemTypeRepository
{
    public void Add(ItemType itemType) => db.ItemTypes.Add(itemType);

    public async Task<IReadOnlyList<ItemType>> ListActiveAsync(CancellationToken ct) =>
        await db.ItemTypes.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.Name).ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct) =>
        db.ItemTypes.AnyAsync(t => t.Code == code, ct);
}
