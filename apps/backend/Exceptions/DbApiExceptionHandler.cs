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
        if (exception is not DbApiException dbApiException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Title = "Database API error",
            Detail = dbApiException.Message,
            Status = (int)dbApiException.StatusCode
        };

        problemDetails.Extensions["Errors"] = dbApiException.ResponseBody;

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}