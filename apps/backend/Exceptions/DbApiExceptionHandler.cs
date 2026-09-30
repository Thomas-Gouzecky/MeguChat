using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class DbApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DbApiExceptionHandler> _logger;

    public DbApiExceptionHandler(
        ILogger<DbApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbApiException dbApiException) // Looks for a DbApiException, if not found, returns false to let other handlers handle it
        {
            return false;
        }

        _logger.LogError(exception, "Database API error occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Title = "Database API error",
            Detail = dbApiException.Message,
            Status = (int)dbApiException.StatusCode
        };

        problemDetails.Extensions["errors"] = dbApiException.ResponseBody;

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}