using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.Identity.Domain.Access;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Identity.Application.Features.Users;

public sealed record UserResponse(
    Guid Id, string UserName, string Email, string FirstName, string? MiddleName, string LastName,
    string FullName, bool IsLocked, IReadOnlyList<string> Roles, IReadOnlyList<Guid> CompanyIds)
{
    public static UserResponse From(UserAccount a) => new(
        a.Id, a.UserName, a.Email, a.FirstName, a.MiddleName, a.LastName, a.FullName,
        a.IsLocked, a.Access.Roles, a.Access.CompanyIds);
}

// ---- List / get ---------------------------------------------------------------------------

public sealed record ListUsersQuery;

public static class ListUsersHandler
{
    public static async Task<IReadOnlyList<UserResponse>> Handle(ListUsersQuery _, IUserAccounts accounts, CancellationToken ct) =>
        (await accounts.ListAsync(ct)).Select(UserResponse.From).ToList();
}

public sealed record GetUserQuery(Guid Id);

public static class GetUserHandler
{
    public static async Task<Result<UserResponse>> Handle(GetUserQuery query, IUserAccounts accounts, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(query.Id, ct);
        return account is null ? Error.NotFound("user.not_found", "User not found.") : UserResponse.From(account);
    }
}

// ---- Availability (used by the register form for live feedback) -----------------------------

public sealed record CheckAvailabilityQuery(string? UserName, string? Email);

public sealed record AvailabilityResponse(bool UserNameTaken, bool EmailTaken);

public static class CheckAvailabilityHandler
{
    public static async Task<AvailabilityResponse> Handle(CheckAvailabilityQuery query, IUserAccounts accounts, CancellationToken ct) =>
        new(
            !string.IsNullOrWhiteSpace(query.UserName) && await accounts.UserNameExistsAsync(query.UserName.Trim(), ct),
            !string.IsNullOrWhiteSpace(query.Email) && await accounts.EmailExistsAsync(query.Email.Trim(), ct));
}

// ---- Register -----------------------------------------------------------------------------

public sealed record RegisterUserCommand(
    string UserName, string Email, string FirstName, string? MiddleName, string LastName,
    string Password, IReadOnlyList<string> Roles, IReadOnlyList<Guid> CompanyIds);

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(50).Matches(@"^[A-Za-z0-9._-]+$");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

public static class RegisterUserHandler
{
    public static async Task<Result<Guid>> Handle(RegisterUserCommand command, IUserAccounts accounts, CancellationToken ct)
    {
        if (await accounts.UserNameExistsAsync(command.UserName.Trim(), ct))
            return Error.Conflict("user.name_taken", $"User name '{command.UserName}' is already taken.");
        if (await accounts.EmailExistsAsync(command.Email.Trim(), ct))
            return Error.Conflict("user.email_taken", $"Email '{command.Email}' is already registered.");

        var access = AccessProfile.Create(command.Roles, command.CompanyIds);
        var user = new NewUserAccount(command.UserName.Trim(), command.Email.Trim(), command.FirstName.Trim(),
            command.MiddleName?.Trim(), command.LastName.Trim(), access);
        return await accounts.CreateAsync(user, command.Password, ct);
    }
}

// ---- Update access ------------------------------------------------------------------------

public sealed record UpdateUserAccessCommand(
    Guid Id, IReadOnlyList<string> Roles, IReadOnlyList<Guid> CompanyIds, bool IsLocked, string? NewPassword);

public sealed class UpdateUserAccessValidator : AbstractValidator<UpdateUserAccessCommand>
{
    public UpdateUserAccessValidator()
    {
        RuleFor(x => x.NewPassword).MinimumLength(8).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.NewPassword));
    }
}

public static class UpdateUserAccessHandler
{
    public static async Task<Result> Handle(UpdateUserAccessCommand command, IUserAccounts accounts, CancellationToken ct)
    {
        var access = AccessProfile.Create(command.Roles, command.CompanyIds);
        return await accounts.UpdateAccessAsync(command.Id, access, command.IsLocked,
            string.IsNullOrEmpty(command.NewPassword) ? null : command.NewPassword, ct);
    }
}
