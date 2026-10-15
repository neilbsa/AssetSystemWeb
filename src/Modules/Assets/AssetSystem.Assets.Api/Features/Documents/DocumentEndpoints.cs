using AssetSystem.BuildingBlocks.Web;
using AssetSystem.SharedKernel.Abstractions;
using AssetSystem.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

using AssetSystem.Assets.Application.Features.Documents;

namespace AssetSystem.Assets.Api.Features.Documents;

internal sealed class DocumentEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets/{id:guid}/documents")
            .WithTags("Assets · Documents")
            .RequireAuthorization(Policies.CompanySelected);

        group.MapPost("/", async (
                Guid id, [Microsoft.AspNetCore.Mvc.FromForm] string documentType, IFormFile file,
                ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            {
                await using var content = file.OpenReadStream();
                return (await bus.InvokeAsync<Result<Guid>>(new UploadAssetDocumentCommand(
                        user.RequireCompany(), id, documentType, file.FileName, file.ContentType, file.Length, content), ct))
                    .ToCreated(docId => $"/api/assets/{id}/documents/{docId}");
            })
            .DisableAntiforgery(); // bearer-token API: there is no cookie to forge

        group.MapGet("/{documentId:guid}", async (Guid id, Guid documentId, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
        {
            var result = await bus.InvokeAsync<Result<DocumentDownload>>(
                new DownloadAssetDocumentQuery(user.RequireCompany(), id, documentId), ct);
            return result.IsSuccess
                ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
                : result.ToHttp();
        });

        group.MapDelete("/{documentId:guid}", async (Guid id, Guid documentId, ICurrentUser user, IMessageBus bus, CancellationToken ct) =>
            (await bus.InvokeAsync<Result>(new RemoveAssetDocumentCommand(user.RequireCompany(), id, documentId), ct)).ToHttp());
    }
}
