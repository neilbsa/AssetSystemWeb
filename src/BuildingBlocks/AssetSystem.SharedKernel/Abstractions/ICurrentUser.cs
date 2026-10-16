namespace AssetSystem.SharedKernel.Abstractions;

/// <summary>The authenticated caller and the company they are currently working in.</summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    string UserName { get; }
    Guid? CompanyId { get; }
    bool IsAdministrator { get; }
}
