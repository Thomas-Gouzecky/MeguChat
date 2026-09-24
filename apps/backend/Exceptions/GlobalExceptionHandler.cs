using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            UnauthenticatedAccessException =>
                new ProblemDetails
                {
                    Title = "User not authenticated",
                    Detail = exception.Message,
                    Status = StatusCodes.Status401Unauthorized
                },

            NotFoundException =>
                new ProblemDetails
                {
                    Title = "Not found",
                    Detail = exception.Message,
                    Status = StatusCodes.Status404NotFound
                },

            ArgumentException =>
                new ProblemDetails
                {
                    Title = "Invalid argument",
                    Detail = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                },
            UnauthorizedAccessException =>
                new ProblemDetails
                {
                    Title = "Unauthorized access",
                    Detail = exception.Message,
                    Status = StatusCodes.Status403Forbidden
                },

            _ =>
                new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError
                }
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}