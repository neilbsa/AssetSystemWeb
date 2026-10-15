using AssetSystem.SharedKernel.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AssetSystem.BuildingBlocks.Web;

/// <summary>Maps validation failures and broken domain invariants to problem details; everything else is a 500.</summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        ProblemDetails problem;

        switch (exception)
        {
            case ValidationException validation:
                problem = new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Validation failed" };
                problem.Extensions["errors"] = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            case DomainException domain:
                problem = new ProblemDetails { Status = StatusCodes.Status422UnprocessableEntity, Title = domain.Message };
                problem.Extensions["code"] = domain.Code;
                break;

            case BadHttpRequestException bad:
                problem = new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = bad.Message };
                break;

            default:
                logger.LogError(exception, "Unhandled exception");
                problem = new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "An unexpected error occurred." };
                break;
        }

        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}
