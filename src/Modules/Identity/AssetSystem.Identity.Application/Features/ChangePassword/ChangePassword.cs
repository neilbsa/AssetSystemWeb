using AssetSystem.Identity.Application.Abstractions;
using AssetSystem.SharedKernel.Results;
using FluentValidation;

namespace AssetSystem.Identity.Application.Features.ChangePassword;

public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(100);
    }
}

public static class ChangePasswordHandler
{
    public static Task<Result> Handle(ChangePasswordCommand command, IUserAccounts accounts, CancellationToken ct) =>
        accounts.ChangePasswordAsync(command.UserId, command.CurrentPassword, command.NewPassword, ct);
}
