public interface IMCPClient
{
    Task ConnectAsync(string host, int port);
    Task ListToolsAsync();
    Task CallToolAsync(string toolName, string[] args);
    Task DisconnectAsync();
}