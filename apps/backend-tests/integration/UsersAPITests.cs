using System.Net;
using System.Net.Http.Json;

namespace backend.Tests;

public class UsersAPITests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersAPITests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllUsers_WithAuthenticatedUser_ReturnsUsers()
    {
        await LoginAsTestUser();

        var response = await _client.GetAsync("/api/users");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();

        Assert.NotNull(users);
        Assert.Contains(users, user => user.Username == "testuser");
        Assert.Contains(users, user => user.Username == "nouser");
        Assert.All(users, user =>
        {
            Assert.False(string.IsNullOrWhiteSpace(user.Id));
            Assert.False(string.IsNullOrWhiteSpace(user.Username));
        });
    }

    [Fact]
    public async Task GetAllUsers_WithoutAuthenticatedUser_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllUsers_ReflectsUsersRegisteredAfterStartup()
    {
        await LoginAsTestUser();

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new
            {
                username = "users-endpoint-test-user",
                password = "TestPassword1!"
            });
        registerResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync("/api/users");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();

        Assert.NotNull(users);
        Assert.Contains(users, user => user.Username == "users-endpoint-test-user");
    }

    private async Task LoginAsTestUser()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                username = "testuser",
                password = "TestPassword1!"
            });

        response.EnsureSuccessStatusCode();
    }
}