using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.SharedKernel.Results;
using Wolverine;

namespace AssetSystem.Identity.Application.Features.Session;

public sealed record GetSessionQuery(Guid UserId, Guid? CompanyId);

public static class GetSessionHandler
{
    public static async Task<Result<SessionResponse>> Handle(
        GetSessionQuery query, IUserAccounts accounts, IMessageBus bus, CancellationToken ct)
    {
        var account = await accounts.FindByIdAsync(query.UserId, ct);
        if (account is null || account.IsLocked) return Error.Forbidden("auth.invalid_user", "Account is not available.");

        var companies = await bus.AccessibleCompaniesAsync(account, ct);
        var current = companies.Any(c => c.Id == query.CompanyId) ? query.CompanyId : null;
        return new SessionResponse(null, null, account.ToInfo(), companies, current);
    }
}
