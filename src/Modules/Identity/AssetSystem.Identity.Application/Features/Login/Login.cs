using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.SharedKernel.Results;
using FluentValidation;
using Wolverine;

namespace AssetSystem.Identity.Application.Features.Login;

public sealed record LoginCommand(string UserName, string Password);

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public static class LoginHandler
{
    public static async Task<Result<SessionResponse>> Handle(
        LoginCommand command, IUserAccounts accounts, ITokenIssuer tokens, IMessageBus bus, CancellationToken ct)
    {
        var (outcome, account) = await accounts.CheckCredentialsAsync(command.UserName.Trim(), command.Password, ct);

        if (outcome == CredentialCheck.LockedOut)
            return Error.Forbidden("auth.locked_out", "This account is locked. Contact an administrator or try again later.");
        if (outcome != CredentialCheck.Valid || account is null)
            return Error.Forbidden("auth.invalid_credentials", "Invalid user name or password.");

        var companies = await bus.AccessibleCompaniesAsync(account, ct);
        if (companies.Count == 0)
            return Error.Forbidden("auth.no_company", "No company is assigned to this account.");

        // A single-company user skips the company picker.
        Guid? companyId = companies.Count == 1 ? companies[0].Id : null;
        var token = tokens.Issue(account, companyId);
        return new SessionResponse(token.Value, token.ExpiresAt, account.ToInfo(), companies, companyId);
    }
}
