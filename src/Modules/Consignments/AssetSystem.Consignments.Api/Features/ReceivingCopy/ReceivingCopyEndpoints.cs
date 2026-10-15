using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Consignments.Application.Features.ReceivingCopy;

namespace AssetSystem.Consignments.Api.Features.ReceivingCopy;

internal sealed class ReceivingCopyEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/consignments/{id:guid}/receiving-copy")
            .WithTags("Consignments")
            .RequireAuthorization(Policies.CompanySelected);

        group.MapPut("/", async (Guid id, IFormFile file, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            {
                await using var content = file.OpenReadStream();
                return (await bus.InvokeAsync<Result>(new UploadReceivingCopyCommand(
                    user.RequireCompany(), id, file.FileName, file.ContentType, file.Length, content), ct)).ToHttp();
            })
            .DisableAntiforgery(); // bearer-token API: there is no cookie to forge

        group.MapGet("/", async (Guid id, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<Result<ReceivingCopyDownload>>(new DownloadReceivingCopyQuery(user.RequireCompany(), id), ct);
            return result.IsSuccess
                ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
                : result.ToHttp();
        });
    }
}
