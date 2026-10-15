using AssetSystem.Organization.Domain.Branches;
using AssetSystem.Organization.Domain.Companies;
using AssetSystem.Organization.Domain.Departments;
using AssetSystem.Organization.Domain.Employees;
using AssetSystem.Organization.Domain.ItemTypes;
using AssetSystem.Organization.Domain.Suppliers;
using AssetSystem.SharedKernel.Abstractions;

namespace AssetSystem.Organization.Application.Abstractions;

public interface IOrganizationUnitOfWork : IUnitOfWork;

public interface ICompanyRepository
{
    void Add(Company company);
    Task<Company?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Company>> ListAsync(CancellationToken ct);
    Task<IReadOnlyList<Company>> GetByIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
}

public interface IBranchRepository
{
    void Add(Branch branch);
    Task<Branch?> GetAsync(Guid companyId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Branch>> ListAsync(Guid companyId, CancellationToken ct);
}

public interface IDepartmentRepository
{
    void Add(Department department);
    Task<Department?> GetAsync(Guid companyId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Department>> ListAsync(Guid companyId, CancellationToken ct);
    Task<bool> CodeExistsAsync(Guid companyId, string code, CancellationToken ct);
}

public interface ISupplierRepository
{
    void Add(Supplier supplier);
    Task<IReadOnlyList<Supplier>> SearchAsync(string? term, int limit, CancellationToken ct);
    Task<Supplier?> FindExactAsync(string name, string address, string? contactNumber, string email, CancellationToken ct);
}

public interface IEmployeeRepository
{
    void Add(Employee employee);
    Task<Employee?> GetAsync(Guid companyId, Guid id, CancellationToken ct);
    Task<Employee?> GetByNumberAsync(Guid companyId, string employeeNumber, CancellationToken ct);
    Task<IReadOnlyList<Employee>> ListAsync(Guid companyId, string? search, CancellationToken ct);
}

public interface IItemTypeRepository
{
    void Add(ItemType itemType);
    Task<IReadOnlyList<ItemType>> ListActiveAsync(CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct);
}
