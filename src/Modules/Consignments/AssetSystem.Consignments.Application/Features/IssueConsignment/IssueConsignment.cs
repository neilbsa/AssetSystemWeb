using AssetSystem.Assets.Contracts;
using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Domain.Consignments;
using AssetSystem.Organization.Contracts;
using AssetSystem.SharedKernel.Results;
using FluentValidation;
using Wolverine;

namespace AssetSystem.Consignments.Application.Features.IssueConsignment;

public sealed record IssueConsignmentCommand(
    Guid CompanyId, Guid EmployeeId, DateOnly IssuedOn, string? Remarks, IReadOnlyList<Guid> ItemIds);

public sealed record IssuedConsignmentResponse(Guid Id, string Number);

public sealed class IssueConsignmentValidator : AbstractValidator<IssueConsignmentCommand>
{
    public IssueConsignmentValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.ItemIds).NotEmpty().WithMessage("Add at least one item.");
        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}

public static class IssueConsignmentHandler
{
    private const string AvailableStatus = "Available";

    public static async Task<Result<IssuedConsignmentResponse>> Handle(
        IssueConsignmentCommand command, IMessageBus bus, IConsignmentRepository consignments,
        IConsignmentNumbering numbering, IConsignmentsUnitOfWork uow, TimeProvider clock, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);

        var employee = await bus.InvokeAsync<EmployeeDto?>(new GetEmployee(command.CompanyId, command.EmployeeId), ct);
        if (employee is null) return Error.NotFound("employee.not_found", "Consignee not found.");

        var itemIds = command.ItemIds.Distinct().ToList();
        var items = await bus.InvokeAsync<IReadOnlyList<ConsignmentItemDto>>(new GetItemsForConsignment(command.CompanyId, itemIds), ct);

        if (items.Count != itemIds.Count)
            return Error.Validation("consignment.unknown_item", "One or more items do not exist.");

        // Early, friendly check. The authoritative one happens when Assets reserves the items (see ConsignmentIssuedHandler).
        var unavailable = items.FirstOrDefault(i => i.Status != AvailableStatus);
        if (unavailable is not null)
            return Error.Conflict("consignment.item_unavailable", $"Item {unavailable.ItemCode} is {unavailable.Status} and cannot be issued.");

        var consignment = Consignment.Issue(
            command.CompanyId, await numbering.NextNumberAsync(command.CompanyId, ct),
            employee.Id, employee.EmployeeNumber, employee.FullName, employee.DepartmentName,
            command.IssuedOn, command.Remarks,
            items.Select(i => new NewConsignmentLine(i.Id, i.ItemCode, i.TypeName, i.Description, i.SerialNumber, i.AssetNumber)).ToList(),
            today);

        consignments.Add(consignment);
        await uow.CommitAsync(ct); // also stores ConsignmentIssuedDomainEvent in the outbox
        return new IssuedConsignmentResponse(consignment.Id, consignment.Number);
    }
}
