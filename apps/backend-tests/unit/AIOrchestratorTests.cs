using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace backend.Tests;

public class AIOrchestratorTests
{
    [Fact]
    public async Task ProcessMessageAsync_WhenMcpIsConnected_SendsMessageAndContextToChatClient()
    {
        var mcpClient = new Mock<IMCPClient>();
        mcpClient.SetupGet(client => client.IsConnected).Returns(true);
        mcpClient
            .Setup(client => client.ListToolsAsync())
            .ReturnsAsync(Array.Empty<ModelContextProtocol.Client.McpClientTool>());

        var expectedResponse = new ChatResponse(
            new ChatMessage(ChatRole.Assistant, "Test response"));
        var chatClient = new Mock<IChatClient>();
        IEnumerable<ChatMessage>? receivedMessages = null;
        ChatOptions? receivedOptions = null;
        chatClient
            .Setup(client => client.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<ChatMessage>, ChatOptions, CancellationToken>(
                (messages, options, _) =>
                {
                    receivedMessages = messages;
                    receivedOptions = options;
                })
            .ReturnsAsync(expectedResponse);

        var orchestrator = CreateOrchestrator(mcpClient, chatClient);
        var userContext = new AIUserContext("user-id", "Test User", "user@example.com");

        var response = await orchestrator.ProcessMessageAsync(
            "Hello",
            userContext,
            "session-id",
            CancellationToken.None);

        Assert.Same(expectedResponse, response);
        var message = Assert.Single(receivedMessages!);
        Assert.Equal(ChatRole.User, message.Role);
        Assert.Equal("Hello", message.Text);
        Assert.Contains("user-id", receivedOptions!.Instructions);
        Assert.Contains("session-id", receivedOptions.Instructions);
        mcpClient.Verify(client => client.ConnectAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenMcpIsDisconnected_ConnectsBeforeRequestingTools()
    {
        var mcpClient = new Mock<IMCPClient>();
        mcpClient.SetupSequence(client => client.IsConnected)
            .Returns(false)
            .Returns(true);
        mcpClient
            .Setup(client => client.ListToolsAsync())
            .ReturnsAsync(Array.Empty<ModelContextProtocol.Client.McpClientTool>());

        var chatClient = new Mock<IChatClient>();
        var expectedResponse = new ChatResponse(
            new ChatMessage(ChatRole.Assistant, "Connected response"));
        chatClient
            .Setup(client => client.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MCP_ENDPOINT"] = "http://localhost:1234/mcp"
            })
            .Build();
        var orchestrator = new AIOrchestrator(
            mcpClient.Object,
            NullLogger<AIOrchestrator>.Instance,
            chatClient.Object,
            configuration);

        await orchestrator.ProcessMessageAsync(
            "Hello",
            new AIUserContext("user-id", "Test User", "user@example.com"),
            "session-id",
            CancellationToken.None);

        mcpClient.Verify(client => client.ConnectAsync("http://localhost:1234/mcp"), Times.Once);
        mcpClient.Verify(client => client.ListToolsAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcessMessageAsync_WhenEndpointIsMissing_ThrowsWithoutCallingChatClient()
    {
        var mcpClient = new Mock<IMCPClient>();
        mcpClient.SetupGet(client => client.IsConnected).Returns(false);
        var chatClient = new Mock<IChatClient>();
        var orchestrator = CreateOrchestrator(
            mcpClient,
            chatClient,
            new ConfigurationBuilder().Build());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => orchestrator.ProcessMessageAsync(
                "Hello",
                new AIUserContext("user-id", "Test User", "user@example.com"),
                "session-id",
                CancellationToken.None));

        Assert.Equal("MCP_ENDPOINT environment variable is missing.", exception.Message);
        chatClient.Verify(
            client => client.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void IsHealthy_ReflectsMcpConnectionState()
    {
        var mcpClient = new Mock<IMCPClient>();
        mcpClient.SetupGet(client => client.IsConnected).Returns(true);
        var orchestrator = CreateOrchestrator(mcpClient, new Mock<IChatClient>());

        Assert.True(orchestrator.isHealthy());

        mcpClient.SetupGet(client => client.IsConnected).Returns(false);

        Assert.False(orchestrator.isHealthy());
    }

    private static AIOrchestrator CreateOrchestrator(
        Mock<IMCPClient> mcpClient,
        Mock<IChatClient> chatClient,
        IConfiguration? configuration = null)
    {
        return new AIOrchestrator(
            mcpClient.Object,
            NullLogger<AIOrchestrator>.Instance,
            chatClient.Object,
            configuration ?? new ConfigurationBuilder().Build());
    }
}
