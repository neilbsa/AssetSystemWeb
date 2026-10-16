using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.SharedKernel.Results;
using Wolverine;

namespace AssetSystem.Identity.Application.Features.SelectCompany;

public sealed record SelectCompanyCommand(Guid UserId, Guid CompanyId);

public static class SelectCompanyHandler
{
    public static async Task<Result<SessionResponse>> Handle(
        SelectCompanyCommand command, IUserAccounts accounts, ITokenIssuer tokens, IMessageBus bus, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(command.UserId, ct);
        if (account is null || account.IsLocked) return Error.Forbidden("auth.invalid_user", "Account is not available.");

        var companies = await bus.AccessibleCompaniesAsync(account, ct);
        if (companies.All(c => c.Id != command.CompanyId))
            return Error.Forbidden("auth.company_not_allowed", "You do not have access to that company.");

        var token = tokens.Issue(account, command.CompanyId);
        return new SessionResponse(token.Value, token.ExpiresAt, account.ToInfo(), companies, command.CompanyId);
    }
}
