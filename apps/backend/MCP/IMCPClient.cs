using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

public interface IMCPClient
{
    Task ConnectAsync(string host, int port);
    ValueTask<IList<McpClientTool>> ListToolsAsync();
    ValueTask<CallToolResult> CallToolAsync(string toolName, Dictionary<string, object?> toolInputs);
    ValueTask DisconnectAsync();
}