using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.Employees;

/// <summary>A person who can be given company assets (the "consignee"). Not the same as a system login.</summary>
public sealed class Employee : AggregateRoot, IAuditable
{
    private Employee() { }

    private Employee(Guid id, Guid companyId, string employeeNumber) : base(id)
    {
        CompanyId = companyId;
        EmployeeNumber = employeeNumber;
    }

    public Guid CompanyId { get; private set; }
    public string EmployeeNumber { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public Guid DepartmentId { get; private set; }
    public Guid BranchId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Employee Register(
        Guid companyId, string employeeNumber, string firstName, string? middleName,
        string lastName, string email, Guid departmentId, Guid branchId)
    {
        var employee = new Employee(NewId(), companyId, Guard.NotBlank(employeeNumber, nameof(EmployeeNumber), 30));
        employee.UpdateDetails(firstName, middleName, lastName, email, departmentId, branchId);
        return employee;
    }

    public void UpdateDetails(
        string firstName, string? middleName, string lastName, string email, Guid departmentId, Guid branchId)
    {
        FirstName = Guard.NotBlank(firstName, nameof(FirstName), 100);
        MiddleName = Guard.OptionalText(middleName, nameof(MiddleName), 100);
        LastName = Guard.NotBlank(lastName, nameof(LastName), 100);
        Email = Guard.NotBlank(email, nameof(Email));
        Guard.That(email.Contains('@'), "Email.invalid", "Email is not a valid address.");
        DepartmentId = departmentId;
        BranchId = branchId;
    }

    public void Deactivate() => IsActive = false;
}
