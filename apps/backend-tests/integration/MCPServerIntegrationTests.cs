using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Protocol;
using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests;

[Trait("Category", "MCPIntegration")]
public class MCPServerIntegrationTests
{
    private const string Endpoint = "http://localhost:8023/mcp";

    [Fact]
    public async Task ListToolsAsync_ReturnsTestTool()
    {
        if (!IsIntegrationEnabled())
        {
            return;
        }

        await using var client = CreateClient();
        await client.ConnectAsync(Endpoint);

        var tools = await client.ListToolsAsync();

        Assert.Contains(tools, tool => tool.Name == "test-tool");
    }

    [Fact]
    public async Task CallToolAsync_TestTool_EchoesMessage()
    {
        if (!IsIntegrationEnabled())
        {
            return;
        }

        await using var client = CreateClient();
        await client.ConnectAsync(Endpoint);

        var result = await client.CallToolAsync(
            "test-tool",
            new Dictionary<string, object?>
            {
                ["message"] = "integration test"
            });

        Assert.False(result.IsError);
        var text = Assert.IsType<TextContentBlock>(Assert.Single(result.Content));
        Assert.Equal("Echo: integration test", text.Text);
    }

    [Fact]
    public async Task AIOrchestrator_WithRealMcpClient_ProvidesServerToolsToChatClient()
    {
        if (!IsIntegrationEnabled())
        {
            return;
        }

        await using var mcpClient = CreateClient();
        var chatClient = new RecordingChatClient();
        var orchestrator = new AIOrchestrator(
            mcpClient,
            NullLogger<AIOrchestrator>.Instance,
            chatClient,
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["MCP_ENDPOINT"] = Endpoint
                })
                .Build());

        await orchestrator.ProcessMessageAsync(
            "Use the test tool",
            new AIUserContext("user-id", "Integration User", "user@example.com"),
            "integration-session",
            CancellationToken.None);

        Assert.Contains(
            chatClient.Options!.Tools!,
            tool => tool is ModelContextProtocol.Client.McpClientTool mcpTool
                && mcpTool.Name == "test-tool");
    }

    private static MCPClient CreateClient()
    {
        return new MCPClient(NullLogger<MCPClient>.Instance);
    }

    private static bool IsIntegrationEnabled()
    {
        var environmentValue = Environment.GetEnvironmentVariable("RUN_MCP_INTEGRATION_TESTS");
        if (environmentValue is not null)
        {
            return IsTrue(environmentValue);
        }

        var testEnvironmentFile = Path.Combine(
            AppContext.BaseDirectory,
            ".env.test");
        if (!File.Exists(testEnvironmentFile))
        {
            return false;
        }

        var setting = File.ReadLines(testEnvironmentFile)
            .Select(line => line.Trim())
            .FirstOrDefault(line =>
                line.StartsWith("RUN_MCP_INTEGRATION_TESTS=", StringComparison.Ordinal));

        return setting is not null
            && IsTrue(setting["RUN_MCP_INTEGRATION_TESTS=".Length..].Trim().Trim('"'));
    }

    private static bool IsTrue(string value)
    {
        return string.Equals(value.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class RecordingChatClient : IChatClient
    {
        public ChatOptions? Options { get; private set; }

        public ChatClientMetadata Metadata => new("MCP integration test");

        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> chatMessages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            Options = options;
            return Task.FromResult(new ChatResponse(
                new ChatMessage(ChatRole.Assistant, "Test response")));
        }

        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> chatMessages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            yield break;
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            return null;
        }

        public void Dispose()
        {
        }
    }
}
