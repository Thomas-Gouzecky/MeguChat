using Microsoft.Extensions.Logging.Abstractions;

namespace backend.Tests;

public class MCPClientTests
{
    [Fact]
    public async Task ListToolsAsync_WhenDisconnected_Throws()
    {
        await using var client = new MCPClient(NullLogger<MCPClient>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.ListToolsAsync());

        Assert.Equal("The MCP client is not connected.", exception.Message);
    }

    [Fact]
    public async Task CallToolAsync_WhenDisconnected_Throws()
    {
        await using var client = new MCPClient(NullLogger<MCPClient>.Instance);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await client.CallToolAsync("test", new Dictionary<string, object?>()));

        Assert.Equal("The MCP client is not connected.", exception.Message);
    }

    [Fact]
    public async Task DisconnectAsync_WhenDisconnected_IsIdempotent()
    {
        await using var client = new MCPClient(NullLogger<MCPClient>.Instance);

        await client.DisconnectAsync();

        Assert.False(client.IsConnected);
    }

    [Fact]
    public async Task ConnectAsync_WithInvalidEndpoint_ThrowsAndLeavesClientDisconnected()
    {
        await using var client = new MCPClient(NullLogger<MCPClient>.Instance);

        await Assert.ThrowsAsync<UriFormatException>(
            () => client.ConnectAsync("not-a-valid-uri"));

        Assert.False(client.IsConnected);
    }
}
