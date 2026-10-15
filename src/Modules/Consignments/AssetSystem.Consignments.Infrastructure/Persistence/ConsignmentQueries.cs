using AssetSystem.Consignments.Application.Abstractions;
using AssetSystem.Consignments.Domain.Consignments;
using AssetSystem.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

public sealed class ConsignmentQueries(ConsignmentsDbContext db) : IConsignmentQueries
{
    public async Task<PagedResult<ConsignmentSummary>> SearchAsync(
        Guid companyId, Guid? consigneeId, string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Consignments.AsNoTracking().Where(c => c.CompanyId == companyId);
        if (consigneeId is not null) query = query.Where(c => c.ConsigneeId == consigneeId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(c =>
                EF.Functions.ILike(c.Number, pattern) ||
                EF.Functions.ILike(c.ConsigneeName, pattern) ||
                EF.Functions.ILike(c.ConsigneeNumber, pattern) ||
                c.Lines.Any(l => EF.Functions.ILike(l.ItemCode, pattern) || EF.Functions.ILike(l.AssetNumber, pattern)));
        }

        var total = await query.CountAsync(ct);
        var rows = await query
            .OrderByDescending(c => c.Number)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new
            {
                c.Id, c.Number, c.ConsigneeId, c.ConsigneeNumber, c.ConsigneeName, c.IssuedOn, c.Status,
                ItemCount = c.Lines.Count(),
                HasCopy = c.ReceivingCopy != null
            })
            .ToListAsync(ct);

        var items = rows.Select(r => new ConsignmentSummary(
            r.Id, r.Number, r.ConsigneeId, r.ConsigneeNumber, r.ConsigneeName, r.IssuedOn, r.Status.ToString(), r.ItemCount, r.HasCopy)).ToList();
        return new PagedResult<ConsignmentSummary>(items, total, page, pageSize);
    }

    public async Task<IReadOnlyList<ConsigneeSummary>> ListConsigneesAsync(Guid companyId, string? search, CancellationToken ct)
    {
        var query = db.Consignments.AsNoTracking().Where(c => c.CompanyId == companyId && c.Status == ConsignmentStatus.Open);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(c => EF.Functions.ILike(c.ConsigneeName, pattern) || EF.Functions.ILike(c.ConsigneeNumber, pattern));
        }

        var rows = await query
            .Select(c => new
            {
                c.ConsigneeId, c.ConsigneeNumber, c.ConsigneeName, c.DepartmentName,
                Active = c.Lines.Count(l => l.Status == LineStatus.Active)
            })
            .ToListAsync(ct);

        return rows
            .GroupBy(r => r.ConsigneeId)
            .Select(g =>
            {
                var latest = g.First();
                return new ConsigneeSummary(g.Key, latest.ConsigneeNumber, latest.ConsigneeName, latest.DepartmentName,
                    g.Count(), g.Sum(r => r.Active));
            })
            .OrderBy(c => c.ConsigneeName)
            .ToList();
    }

    public async Task<IReadOnlyList<ItemHistoryEntry>> ItemHistoryAsync(Guid companyId, Guid itemId, CancellationToken ct)
    {
        var rows = await db.Consignments.AsNoTracking()
            .Where(c => c.CompanyId == companyId)
            .SelectMany(c => c.Lines.Where(l => l.ItemId == itemId)
                .Select(l => new { c.Id, c.Number, c.ConsigneeName, c.IssuedOn, c.Status, LineStatus = l.Status }))
            .OrderByDescending(r => r.IssuedOn).ThenByDescending(r => r.Number)
            .ToListAsync(ct);

        return rows.Select(r => new ItemHistoryEntry(
            r.Id, r.Number, r.ConsigneeName, r.IssuedOn, r.Status.ToString(), r.LineStatus.ToString())).ToList();
    }
}
