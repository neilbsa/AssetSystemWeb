namespace AssetSystem.Organization.Contracts;

// Public surface of the Organization module. Other modules talk to it ONLY through these
// messages (sent with Wolverine's IMessageBus), never by referencing its domain or database.

public sealed record CompanyDto(Guid Id, string Code, string Name, string AssetTag);

public sealed record BranchDto(Guid Id, Guid CompanyId, string Name, string AssetTag);

public sealed record ItemTypeDto(string Code, string Name);

public sealed record EmployeeDto(
    Guid Id, string EmployeeNumber, string FullName, string Email, Guid DepartmentId, string DepartmentName);

/// <summary>Companies by id (used by Identity to show the companies a user may work in).</summary>
public sealed record GetCompaniesByIds(IReadOnlyCollection<Guid> Ids);

/// <summary>A company's branch, or null when it does not exist in that company.</summary>
public sealed record GetBranch(Guid CompanyId, Guid BranchId);

public sealed record GetCompany(Guid CompanyId);

/// <summary>Every active company (administrators may work in all of them).</summary>
public sealed record GetActiveCompanies;

public sealed record GetActiveItemTypes;

public sealed record GetEmployee(Guid CompanyId, Guid EmployeeId);

/// <summary>Finds a supplier with identical details or creates it; returns the supplier id.</summary>
public sealed record EnsureSupplier(string Name, string Address, string? ContactNumber, string Email);
