using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Net;

var hubUrl = "http://localhost:5192/hubs/chat";
const string apiBaseUrl = "http://localhost:5192/api/";

var cookieContainer = new CookieContainer();
var handler = new HttpClientHandler
{
    CookieContainer = cookieContainer,
    UseCookies = true
};

using var http = new HttpClient(handler) { BaseAddress = new Uri(apiBaseUrl) };

Console.Write("Username: ");
var username = Console.ReadLine() ?? string.Empty;

Console.Write("Password: ");
var password = Authentication.ReadPassword();

using var loginResponse = await http.PostAsJsonAsync("auth/login", new
{
    Username = username,
    Password = password
});

if (!loginResponse.IsSuccessStatusCode)
{
    var error = await loginResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"Login failed ({(int)loginResponse.StatusCode}): {error}");
    return;
}

Console.WriteLine("Login succeeded.");

using var currentUserResponse = await http.GetAsync("auth/me");
if (!currentUserResponse.IsSuccessStatusCode)
{
    var error = await currentUserResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"Cookie authentication check failed ({(int)currentUserResponse.StatusCode}): {error}");
    return;
}

var currentUser = await currentUserResponse.Content.ReadFromJsonAsync<CurrentUserDto>();
Console.WriteLine($"Authenticated as {currentUser?.Username ?? "unknown user"}.");

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl, options => options.Cookies = cookieContainer)
    .WithAutomaticReconnect()
    .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Trace))
    .Build();

// Register the handler before connecting.
connection.On<MessageDto>("ReceiveMessage", message =>
{
    Console.WriteLine($"Received: {message}");
});

try
{
    await connection.StartAsync();
    Console.WriteLine("Connected to ChatHub!");

    Console.WriteLine("Commands:");
    Console.WriteLine("  join <groupchatId>");
    Console.WriteLine("  send <groupchatId> <message>");
    Console.WriteLine("  exit");

    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            continue;

        if (input == "exit")
            break;

        var parts = input.Split(' ', 3);

        try
        {
            switch (parts[0])
            {
                case "join" when parts.Length >= 2 && int.TryParse(parts[1], out var groupchatId):
                    await connection.InvokeAsync(
                        "JoinGroupChat", groupchatId);
                    Console.WriteLine("Join requested.");
                    break;

                case "send" when parts.Length == 3 && int.TryParse(parts[1], out var groupchatId):
                    MessageCreationRequestDto messageRequest = new MessageCreationRequestDto
                    {
                        Content = parts[2]
                    };
                    await connection.InvokeAsync(
                        "SendMessage", groupchatId, messageRequest);
                    Console.WriteLine("Send requested.");
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hub invocation failed: {ex}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");
}
finally
{
    await connection.StopAsync();
    await connection.DisposeAsync();
}

public sealed record CurrentUserDto(string UserId, string Username);
