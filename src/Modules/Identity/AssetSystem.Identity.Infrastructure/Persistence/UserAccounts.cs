using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.Identity.Domain.Access;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Identity.Infrastructure.Persistence;

public sealed class UserAccounts(UserManager<AppUser> users) : IUserAccounts
{
    public async Task<UserAccount?> FindByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await users.Users.AsNoTracking().Include(u => u.Companies).FirstOrDefaultAsync(u => u.Id == id, ct);
        return user is null ? null : await MapAsync(user);
    }

    public async Task<IReadOnlyList<UserAccount>> ListAsync(CancellationToken ct)
    {
        var list = await users.Users.AsNoTracking().Include(u => u.Companies).OrderBy(u => u.UserName).ToListAsync(ct);
        var result = new List<UserAccount>(list.Count);
        foreach (var user in list) result.Add(await MapAsync(user));
        return result;
    }

    public async Task<(CredentialCheck Outcome, UserAccount? Account)> CheckCredentialsAsync(
        string userName, string password, CancellationToken ct)
    {
        var user = await users.Users.Include(u => u.Companies).FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant(), ct);
        if (user is null) return (CredentialCheck.Invalid, null);

        if (await users.IsLockedOutAsync(user)) return (CredentialCheck.LockedOut, null);

        if (!await users.CheckPasswordAsync(user, password))
        {
            await users.AccessFailedAsync(user); // counts toward lockout
            return (await users.IsLockedOutAsync(user) ? CredentialCheck.LockedOut : CredentialCheck.Invalid, null);
        }

        await users.ResetAccessFailedCountAsync(user);
        return (CredentialCheck.Valid, await MapAsync(user));
    }

    public Task<bool> UserNameExistsAsync(string userName, CancellationToken ct) =>
        users.Users.AnyAsync(u => u.NormalizedUserName == userName.ToUpperInvariant(), ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        users.Users.AnyAsync(u => u.NormalizedEmail == email.ToUpperInvariant(), ct);

    public async Task<Result<Guid>> CreateAsync(NewUserAccount account, string password, CancellationToken ct)
    {
        var user = new AppUser
        {
            Id = Guid.CreateVersion7(),
            UserName = account.UserName,
            Email = account.Email,
            FirstName = account.FirstName,
            MiddleName = account.MiddleName,
            LastName = account.LastName,
            LockoutEnabled = true,
            Companies = account.Access.CompanyIds.Select(c => new UserCompanyAccess { CompanyId = c }).ToList()
        };

        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded) return ToError(created);

        var roles = await users.AddToRolesAsync(user, account.Access.Roles);
        if (!roles.Succeeded) return ToError(roles);

        return user.Id;
    }

    public async Task<Result> UpdateAccessAsync(Guid id, AccessProfile access, bool isLocked, string? newPassword, CancellationToken ct)
    {
        var user = await users.Users.Include(u => u.Companies).FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return Error.NotFound("user.not_found", "User not found.");

        // Roles
        var current = await users.GetRolesAsync(user);
        var toRemove = current.Except(access.Roles).ToList();
        var toAdd = access.Roles.Except(current).ToList();
        if (toRemove.Count > 0) await users.RemoveFromRolesAsync(user, toRemove);
        if (toAdd.Count > 0) await users.AddToRolesAsync(user, toAdd);

        // Companies
        user.Companies.RemoveAll(c => !access.CompanyIds.Contains(c.CompanyId));
        foreach (var companyId in access.CompanyIds.Where(c => user.Companies.All(x => x.CompanyId != c)))
            user.Companies.Add(new UserCompanyAccess { UserId = user.Id, CompanyId = companyId });

        // Lock / unlock
        await users.SetLockoutEnabledAsync(user, true);
        await users.SetLockoutEndDateAsync(user, isLocked ? DateTimeOffset.MaxValue : null);
        if (!isLocked) await users.ResetAccessFailedCountAsync(user);

        var updated = await users.UpdateAsync(user);
        if (!updated.Succeeded) return ToError(updated);

        if (newPassword is not null)
        {
            var token = await users.GeneratePasswordResetTokenAsync(user);
            var reset = await users.ResetPasswordAsync(user, token, newPassword);
            if (!reset.Succeeded) return ToError(reset);
        }

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return Error.NotFound("user.not_found", "User not found.");

        var changed = await users.ChangePasswordAsync(user, currentPassword, newPassword);
        return changed.Succeeded ? Result.Success() : ToError(changed);
    }

    private async Task<UserAccount> MapAsync(AppUser user)
    {
        var roles = await users.GetRolesAsync(user);
        var locked = await users.IsLockedOutAsync(user);
        var access = AccessProfile.Restore(roles, user.Companies.Select(c => c.CompanyId));
        return new UserAccount(user.Id, user.UserName!, user.Email!, user.FirstName, user.MiddleName, user.LastName, locked, access);
    }

    private static Error ToError(IdentityResult result) =>
        Error.Validation("identity.failed", string.Join(" ", result.Errors.Select(e => e.Description)));
}
