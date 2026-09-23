using System.Net.Http.Json;

namespace backend.Tests;

public class GroupChatsAPITests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GroupChatsAPITests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUserGroupChats_ReturnsSuccess()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode, await loginResponse.Content.ReadAsStringAsync());

        // Act
        var response = await _client.GetAsync("/api/groupchats");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<GroupChatResponseDto>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetCurrentUserGroupChats_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Arrange
        var unauthenticatedClient = new HttpClient(); // Create a new HttpClient without authentication

        // Act
        var response = await unauthenticatedClient.GetAsync("https://localhost:5001/api/groupchats");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

}