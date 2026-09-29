using System.Net;

public class DbApiException : Exception
{
    public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
    public DatabaseErrorDto ResponseBody { get; } = new DatabaseErrorDto();
    public DbApiException(HttpStatusCode statusCode, DatabaseErrorDto responseBody)
        : base($"Database API returned an error with status code {(int)statusCode} ({statusCode}).")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}