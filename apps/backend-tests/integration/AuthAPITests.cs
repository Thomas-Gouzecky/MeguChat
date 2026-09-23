using System.Net.Http.Json;

namespace backend.Tests;

public class AuthAPITests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAPITests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode, responseBody);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSetCookieHeader()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Set-Cookie", response.Headers.ToString());
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Register_WithNewUser_ReturnsSuccess()
    {
        // Arrange
        var registerRequest = new
        {
            username = "newuser",
            password = "NewPassword1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode, responseBody);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Register_WithExistingUser_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            username = "testuser", // This user already exists in the test setup
            password = "TestPassword1!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            username = "weakpassworduser",
            password = "123" // Weak password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Logout_ReturnsSuccess()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Log in first to establish a session
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode);

        // Act
        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);
        var responseBody = await logoutResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.True(logoutResponse.IsSuccessStatusCode, responseBody);
        var result = await logoutResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Logout_WithLogin_RemovesCookieInHeader()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Log in first to establish a session
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode);

        // Act
        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);

        // Assert
        Assert.True(logoutResponse.IsSuccessStatusCode);
        Assert.Contains("Set-Cookie", logoutResponse.Headers.ToString());
    }

    [Fact]
    public async Task Logout_WithoutLogin_IsRejected()
    {
        // Act
        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);

        // Assert
        Assert.False(logoutResponse.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_WithAuthenticatedUser_ReturnsUsername()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Log in first to establish a session
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode);

        // Act
        var meResponse = await _client.GetAsync("/api/auth/me");
        var responseBody = await meResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.True(meResponse.IsSuccessStatusCode, responseBody);
        var result = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>();
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutAuthenticatedUser_ReturnsUnauthorized()
    {
        // Act
        var meResponse = await _client.GetAsync("/api/auth/me");
        var responseBody = await meResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_AfterLogout_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };

        // Log in first to establish a session
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode);

        // Log out to clear the session
        var logoutResponse = await _client.PostAsync("/api/auth/logout", null);
        Assert.True(logoutResponse.IsSuccessStatusCode);

        // Act
        var meResponse = await _client.GetAsync("/api/auth/me");
        var responseBody = await meResponse.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, meResponse.StatusCode);
    }

    private sealed record CurrentUserResponse(string Username);
}