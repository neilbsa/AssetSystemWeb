using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Contracts;
using AssetSystem.SharedKernel.Results;

namespace AssetSystem.Consignments.Application.Features.Queries;

// ---- List / search ------------------------------------------------------------------------

public sealed record ListConsignmentsQuery(Guid CompanyId, Guid? ConsigneeId, string? Search, int? Page, int? PageSize);

public static class ListConsignmentsHandler
{
    public static Task<PagedResult<ConsignmentSummary>> Handle(ListConsignmentsQuery q, IConsignmentQueries queries, CancellationToken ct)
    {
        var (page, size) = Paging.Normalize(q.Page, q.PageSize);
        return queries.SearchAsync(q.CompanyId, q.ConsigneeId, q.Search, page, size, ct);
    }
}

/// <summary>"Who holds what": one row per consignee.</summary>
public sealed record ListConsigneesQuery(Guid CompanyId, string? Search);

public static class ListConsigneesHandler
{
    public static Task<IReadOnlyList<ConsigneeSummary>> Handle(ListConsigneesQuery q, IConsignmentQueries queries, CancellationToken ct) =>
        queries.ListConsigneesAsync(q.CompanyId, q.Search, ct);
}

// ---- Details ------------------------------------------------------------------------------

public sealed record GetConsignmentQuery(Guid CompanyId, Guid Id);

public sealed record ConsignmentLineResponse(
    Guid ItemId, string ItemCode, string TypeName, string Description, string SerialNumber, string AssetNumber, string Status);

public sealed record ReceivingCopyResponse(string FileName, string ContentType, long SizeBytes);

public sealed record ConsignmentDetailsResponse(
    Guid Id, string Number, Guid ConsigneeId, string ConsigneeNumber, string ConsigneeName, string? DepartmentName,
    DateOnly IssuedOn, string? Remarks, string Status, string? CancellationReason,
    ReceivingCopyResponse? ReceivingCopy, IReadOnlyList<ConsignmentLineResponse> Lines,
    DateTimeOffset CreatedAt, string CreatedBy);

public static class GetConsignmentHandler
{
    public static async Task<Result<ConsignmentDetailsResponse>> Handle(
        GetConsignmentQuery q, IConsignmentRepository consignments, CancellationToken ct)
    {
        var c = await consignments.GetAsync(q.CompanyId, q.Id, ct);
        if (c is null) return Error.NotFound("consignment.not_found", "Consignment not found.");

        return new ConsignmentDetailsResponse(
            c.Id, c.Number, c.ConsigneeId, c.ConsigneeNumber, c.ConsigneeName, c.DepartmentName,
            c.IssuedOn, c.Remarks, c.Status.ToString(), c.CancellationReason,
            c.ReceivingCopy is null ? null : new ReceivingCopyResponse(c.ReceivingCopy.FileName, c.ReceivingCopy.ContentType, c.ReceivingCopy.SizeBytes),
            c.Lines.OrderBy(l => l.ItemCode)
                .Select(l => new ConsignmentLineResponse(l.ItemId, l.ItemCode, l.TypeName, l.Description, l.SerialNumber, l.AssetNumber, l.Status.ToString()))
                .ToList(),
            c.CreatedAt, c.CreatedBy);
    }
}

// ---- Cross-module query (see Consignments.Contracts) ------------------------------------------

public static class GetItemConsignmentHistoryHandler
{
    public static async Task<IReadOnlyList<ItemConsignmentDto>> Handle(
        GetItemConsignmentHistory q, IConsignmentQueries queries, CancellationToken ct) =>
        (await queries.ItemHistoryAsync(q.CompanyId, q.ItemId, ct))
            .Select(h => new ItemConsignmentDto(h.ConsignmentId, h.Number, h.ConsigneeName, h.IssuedOn, h.ConsignmentStatus, h.LineStatus))
            .ToList();
}
