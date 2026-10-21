using DotNetEnv;
using Anthropic;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var envPath = Path.Combine(
    AppContext.BaseDirectory,
    ".env.local");

Env.Load(envPath);

var builder = Host.CreateApplicationBuilder(args);

var apiKey = builder.Configuration["Anthropic_API_KEY"]
    ?? throw new InvalidOperationException(
        "Anthropic API key is missing.");

var claudeClient = new AnthropicClient
{
    ApiKey = apiKey
};

builder.Services.AddSingleton<IChatClient>(
    claudeClient
        .AsIChatClient("claude-sonnet-5-5")
        .AsBuilder()
        .UseFunctionInvocation()
        .Build());

builder.Services.AddScoped<IAIOrchestrator, AIOrchestrator>();
builder.Services.AddScoped<IMCPClient, MCPClient>();

using var host = builder.Build();
await using var scope = host.Services.CreateAsyncScope();

var orchestrator =
    scope.ServiceProvider.GetRequiredService<IAIOrchestrator>();

Console.Write("Message: ");
var message = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(message))
{
    var response = await orchestrator.ProcessMessageAsync(
        message,
        "test-user",
        "test-session",
        CancellationToken.None);

    Console.WriteLine($"AI: {response.Text}");
}