using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace backend.Tests;

public class AuthTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManager = CreateUserManagerMock();
    private readonly Mock<SignInManager<IdentityUser>> _signInManager;
    private readonly IAuthService _authService;

    public AuthTests()
    {
        _signInManager = CreateSignInManagerMock(_userManager.Object);
        _authService = new AuthService(_signInManager.Object, _userManager.Object);
    }

    [Fact]
    public async Task Login_WithExistingUser_IsSuccessful()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";
        _userManager
            .Setup(manager => manager.FindByNameAsync(username))
            .ReturnsAsync(new IdentityUser { UserName = username });
        _signInManager
            .Setup(manager => manager.PasswordSignInAsync(username, password, false, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        Assert.True(result.IsSuccess);
        _userManager.Verify(manager => manager.FindByNameAsync(username), Times.Once);
    }

    [Fact]
    public async Task Login_WithMissingUser_FailsWithoutSigningIn()
    {
        // Arrange
        var username = "missinguser";
        var password = "testpassword";
        _userManager
            .Setup(manager => manager.FindByNameAsync(username))
            .ReturnsAsync((IdentityUser?)null);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        _signInManager.Verify(
            manager => manager.PasswordSignInAsync(username, password, false, false),
            Times.Never);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Fails()
    {
        // Arrange
        var username = "testuser";
        var password = "invalidpassword";
        _userManager
            .Setup(manager => manager.FindByNameAsync(username))
            .ReturnsAsync(new IdentityUser { UserName = username });
        _signInManager
            .Setup(manager => manager.PasswordSignInAsync(username, password, false, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        return new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Mock<SignInManager<IdentityUser>> CreateSignInManagerMock(
        UserManager<IdentityUser> userManager)
    {
        return new Mock<SignInManager<IdentityUser>>(
            userManager,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
            Options.Create(new IdentityOptions()),
            Mock.Of<ILogger<SignInManager<IdentityUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<IdentityUser>>());
    }
}