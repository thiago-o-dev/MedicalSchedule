using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.ServiceDefaults.ProblemDetails;

public sealed class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, type) = exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                "https://tools.ietf.org/html/rfc7231#section-6.5.4"),

            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://tools.ietf.org/html/rfc7231#section-6.5.8"),

            DomainValidationException => (
                StatusCodes.Status400BadRequest,
                $"Invalid request: {exception.Message}",
                "https://tools.ietf.org/html/rfc7231#section-6.5.1"),

            BusinessLogicException => (
                StatusCodes.Status422UnprocessableEntity,
                $"Business rule violation: {exception.Message}",
                "https://tools.ietf.org/html/rfc4918#section-11.2"),

            _ => (0, string.Empty, string.Empty)
        };

        if (status == 0)
        {
            logger.LogError(exception, "Unhandled exception");
            return false;
        }

        logger.LogWarning(exception, "Domain exception: {Title}", title);

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = status,
                Title = title,
                Type = type,
                Detail = exception.Message,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier,
                    ["exceptionType"] = exception.GetType().Name
                }
            }
        });
    }
}
