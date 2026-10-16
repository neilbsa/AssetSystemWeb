namespace AssetSystem.SharedKernel.Domain;

/// <summary>Base class for anything with an identity. Equality is identity based.</summary>
public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id) => Id = id;

    /// <summary>Required by EF Core.</summary>
    protected Entity() { }

    public Guid Id { get; protected set; }

    public bool Equals(Entity? other) =>
        other is not null && GetType() == other.GetType() && Id == other.Id;

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    /// <summary>Time-ordered GUIDs keep index inserts sequential in PostgreSQL.</summary>
    protected static Guid NewId() => Guid.CreateVersion7();
}
