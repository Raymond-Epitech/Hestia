using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using System.Text.Json;

namespace Api.ErrorHandler;

public class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        var status = exception switch
        {
            InvalidEntityException => StatusCodes.Status422UnprocessableEntity,
            NotFoundException => StatusCodes.Status404NotFound,
            AlreadyExistException => StatusCodes.Status409Conflict,
            InvalidTokenException => StatusCodes.Status401Unauthorized,
            ContextException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        var title = status switch
        {
            422 => "Invalid input data",
            404 => "Resource not found",
            409 => "Conflict with existing resource",
            401 => "Invalid access writes or token",
            _ => "An unexpected server error occurred"
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = status,
            Instance = context.Request.Path
        };

        logger.LogError(exception, $"Handled exception with status {status}");

        context.Response.StatusCode = status;

        if (!context.Response.HasStarted)
        {
            context.Response.ContentType = "application/problem+json";
        }

        var problemContext = new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problemDetails,
            Exception = exception
        };

        var writers = context.RequestServices.GetServices<IProblemDetailsWriter>();

        foreach (var writer in writers)
        {
            if (writer.CanWrite(problemContext))
            {
                await writer.WriteAsync(problemContext);
                return true;
            }
        }

        // Fallback JSON brut
        logger.LogWarning("Fallback JSON: no ProblemDetailsWriter matched the current context.");
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));

        return true;
    }
}
