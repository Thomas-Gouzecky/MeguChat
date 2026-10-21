using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

public interface IMCPClient
{
    bool IsConnected { get; }
    Task ConnectAsync(string endpoint);
    ValueTask<IList<McpClientTool>> ListToolsAsync();
    ValueTask<CallToolResult> CallToolAsync(string toolName, Dictionary<string, object?> toolInputs);
    ValueTask DisconnectAsync();
}