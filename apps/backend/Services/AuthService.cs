using Microsoft.AspNetCore.Identity;

public sealed class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return AuthResult.Failure("Invalid credentials");
        }

        var result = await _signInManager.PasswordSignInAsync(
            username,
            password,
            isPersistent: false,
            lockoutOnFailure: false);

        return result.Succeeded
            ? AuthResult.Success()
            : AuthResult.Failure("Invalid credentials");
    }

    public async Task<AuthResult> RegisterAsync(string username, string password)
    {
        var user = new ApplicationUser { UserName = username };
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? AuthResult.Success()
            : AuthResult.Failure(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task<AuthResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return AuthResult.Success();
    }
}