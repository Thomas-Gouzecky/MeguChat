public sealed record AuthError(
    string Code,
    string Description,
    string InputField = "general");

public sealed record AuthResult(
    bool IsSuccess,
    string? ErrorMessage = null,
    IReadOnlyCollection<AuthError>? Errors = null)
{
    public static AuthResult Success() => new(true, Errors: Array.Empty<AuthError>());

    public static AuthResult Failure(string errorMessage) =>
        new(false, errorMessage, Array.Empty<AuthError>());

    public static AuthResult Failure(IEnumerable<AuthError> errors) =>
        new(
            false,
            string.Join(", ", errors.Select(error => error.Description)),
            errors.ToArray());
}