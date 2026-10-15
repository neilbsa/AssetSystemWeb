using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.Departments;

public sealed class Department : AggregateRoot, IAuditable
{
    private Department() { }

    private Department(Guid id, Guid companyId, string code, string name) : base(id)
    {
        CompanyId = companyId;
        Code = code;
        Name = name;
    }

    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Department Create(Guid companyId, string code, string name) =>
        new(NewId(), companyId, Guard.NotBlank(code, nameof(Code), 20), Guard.NotBlank(name, nameof(Name)));

    public void Rename(string name) => Name = Guard.NotBlank(name, nameof(Name));
    public void Deactivate() => IsActive = false;
}
