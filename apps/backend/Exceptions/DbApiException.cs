using System.Net;

public class DbApiException : Exception
{
    public HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
    public string ResponseBody { get; } = string.Empty;
    public DbApiException(HttpStatusCode statusCode, string responseBody)
        : base($@"Status Code: {statusCode}, Response Body: {responseBody}")
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}