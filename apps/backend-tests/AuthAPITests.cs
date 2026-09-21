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
}