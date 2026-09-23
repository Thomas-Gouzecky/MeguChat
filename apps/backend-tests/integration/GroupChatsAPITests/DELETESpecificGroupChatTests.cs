using System.Net;
using System.Net.Http.Json;

public class DELETESpecificGroupChatTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DELETESpecificGroupChatTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteSpecificGroupChat_ReturnsNoContent_WhenGroupChatExists()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Create a new group chat
        var newGroupChatRequest = new CreateGroupChatRequestDto
        {
            Name = "Test Group Chat",
            UserIds = new List<string> { "user1", "user2" }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/groupchats", newGroupChatRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdGroupChat = await createResponse.Content.ReadFromJsonAsync<GroupChatResponseDto>();

        var groupChatId = createdGroupChat?.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/groupchats/{groupChatId}");

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    }
}