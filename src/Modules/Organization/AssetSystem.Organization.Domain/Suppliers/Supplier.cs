using AssetSystem.SharedKernel.Domain;

namespace AssetSystem.Organization.Domain.Suppliers;

public sealed class Supplier : AggregateRoot, IAuditable
{
    private Supplier() { }

    private Supplier(Guid id, string name, string address, string? contactNumber, string email) : base(id)
    {
        Name = name;
        Address = address;
        ContactNumber = contactNumber;
        Email = email;
    }

    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string? ContactNumber { get; private set; }
    public string Email { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public static Supplier Create(string name, string address, string? contactNumber, string email) =>
        new(NewId(), Guard.NotBlank(name, nameof(Name)), Guard.NotBlank(address, nameof(Address)),
            Guard.OptionalText(contactNumber, nameof(ContactNumber), 50), Guard.NotBlank(email, nameof(Email)));
}
