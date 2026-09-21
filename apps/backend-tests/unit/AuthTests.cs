using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace backend.Tests;

public class AuthTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager = CreateUserManagerMock();
    private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
    private readonly IAuthService _authService;
    private readonly ClaimsPrincipal _currentPrincipal;

    public AuthTests()
    {
        _currentPrincipal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "user-id") },
                "TestAuthentication"));
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = _currentPrincipal }
        };
        _signInManager = CreateSignInManagerMock(_userManager.Object, httpContextAccessor);
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
            .ReturnsAsync(new ApplicationUser { UserName = username });
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
            .ReturnsAsync((ApplicationUser?)null);

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
            .ReturnsAsync(new ApplicationUser { UserName = username });
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
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.True(result.IsSuccess);
        _userManager.Verify(
            manager => manager.CreateAsync(It.Is<ApplicationUser>(u => u.UserName == username), password),
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
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), password))
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
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), password))
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
            .Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>(), password))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _authService.RegisterAsync(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Password must be at least 6 characters", result.ErrorMessage);
    }

    [Fact]
    public async Task Logout_IsSuccessful()
    {
        // Act
        var result = await _authService.LogoutAsync();

        // Assert
        Assert.True(result.IsSuccess);
        _signInManager.Verify(manager => manager.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task Logout_WhenCalledMultipleTimes_IsSuccessful()
    {
        // Act
        var result1 = await _authService.LogoutAsync();
        var result2 = await _authService.LogoutAsync();

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        _signInManager.Verify(manager => manager.SignOutAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUser_WhenAuthenticated()
    {
        // Arrange
        var username = "testuser";
        _userManager
            .Setup(manager => manager.GetUserAsync(_currentPrincipal))
            .ReturnsAsync(new ApplicationUser { UserName = username });

        // Act
        var result = await _authService.GetCurrentUserAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(username, result?.UserName);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsNull_WhenNotAuthenticated()
    {
        // Arrange
        var unauthenticatedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext { User = unauthenticatedPrincipal }
        };
        var signInManagerMock = CreateSignInManagerMock(_userManager.Object, httpContextAccessor);
        var authService = new AuthService(signInManagerMock.Object, _userManager.Object);

        // Act
        var result = await authService.GetCurrentUserAsync();

        // Assert
        Assert.Null(result);
    }

    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        return new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock(
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        return new Mock<SignInManager<ApplicationUser>>(
            userManager,
            httpContextAccessor,
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Options.Create(new IdentityOptions()),
            Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
            Mock.Of<IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<ApplicationUser>>());
    }
}