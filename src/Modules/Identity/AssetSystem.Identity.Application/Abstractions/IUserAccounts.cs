using AssetSystem.Identity.Domain.Access;
using AssetSystem.SharedKernel.Results;

namespace AssetSystem.Identity.Application.Abstractions;

public sealed record UserAccount(
    Guid Id, string UserName, string Email, string FirstName, string? MiddleName, string LastName,
    bool IsLocked, AccessProfile Access)
{
    public string FullName => string.Join(' ', new[] { FirstName, MiddleName, LastName }.Where(p => !string.IsNullOrWhiteSpace(p)));
}

public sealed record NewUserAccount(
    string UserName, string Email, string FirstName, string? MiddleName, string LastName, AccessProfile Access);

public enum CredentialCheck { Valid, Invalid, LockedOut }

/// <summary>Everything the Identity use cases need from the user store (ASP.NET Core Identity in Infrastructure).</summary>
public interface IUserAccounts
{
    Task<UserAccount?> FindByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<UserAccount>> ListAsync(CancellationToken ct);

    /// <summary>Verifies the password and applies the lockout policy.</summary>
    Task<(CredentialCheck Outcome, UserAccount? Account)> CheckCredentialsAsync(string userName, string password, CancellationToken ct);

    Task<bool> UserNameExistsAsync(string userName, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);

    Task<Result<Guid>> CreateAsync(NewUserAccount user, string password, CancellationToken ct);
    Task<Result> UpdateAccessAsync(Guid id, AccessProfile access, bool isLocked, string? newPassword, CancellationToken ct);
    Task<Result> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken ct);
}

public sealed record IssuedToken(string Value, DateTimeOffset ExpiresAt);

public interface ITokenIssuer
{
    IssuedToken Issue(UserAccount account, Guid? companyId);
}
