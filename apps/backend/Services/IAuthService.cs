public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password);
}