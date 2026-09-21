using Microsoft.AspNetCore.Identity;

public sealed class AuthService : IAuthService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
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
}