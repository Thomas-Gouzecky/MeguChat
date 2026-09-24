public sealed record AuthResult(bool IsSuccess, string? ErrorMessage = null)
{
    public static AuthResult Success() => new(true);

    public static AuthResult Failure(string errorMessage) => new(false, errorMessage);
}