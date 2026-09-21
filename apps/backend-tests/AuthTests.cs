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

    [Fact]
    public async Task Register_WithNewUser_IsSuccessful()
    {
        // Arrange
        var username = "newuser";
        var password = "newpassword";
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.True(result.IsSuccess);
        _userManager.Verify(
            manager => manager.CreateAsync(It.Is<IdentityUser>(u => u.UserName == username), password),
            Times.Once);
    }

    [Fact]
    public async Task Register_WithExistingUser_Fails()
    {
        // Arrange
        var username = "existinguser";
        var password = "password";
        var identityError = new IdentityError { Description = "User already exists" };
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("User already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task Register_WithMultipleErrors_FailsWithCombinedErrorMessage()
    {
        // Arrange
        var username = "user";
        var password = "password";
        var identityErrors = new[]
        {
            new IdentityError { Description = "Error 1" },
            new IdentityError { Description = "Error 2" }
        };
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Error 1, Error 2", result.ErrorMessage);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_Fails()
    {
        // Arrange
        var username = "user";
        var password = "short";
        var identityError = new IdentityError { Description = "Password must be at least 6 characters" };
        _userManager
            .Setup(manager => manager.CreateAsync(It.IsAny<IdentityUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Password must be at least 6 characters", result.ErrorMessage);
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