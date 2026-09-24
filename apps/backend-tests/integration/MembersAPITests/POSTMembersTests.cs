using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;

public class POSTMembersTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public POSTMembersTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.ResetGroupChatState();
    }

    [Fact]
    public async Task AddMemberToGroupChat_ShouldReturnSuccess()
    {
        // Login
        var loginRequest = new
        {
            username = "testuser",
            password = "TestPassword1!"
        };
        await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Arrange
        var groupChatId = 1; // Replace with a valid group chat ID
        var requestDto = new AddMemberRequestDto
        {
            UserId = new List<string> { "nouser" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync($"/api/groupchats/{groupChatId}/members", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var addedMembers = JsonConvert.DeserializeObject<IEnumerable<MemberResponseDto>>(responseString);

        Assert.NotNull(addedMembers);
        Assert.Equal(requestDto.UserId.Count(), addedMembers.Count());
    }
}