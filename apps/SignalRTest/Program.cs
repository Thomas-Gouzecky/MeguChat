// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.SignalR.Client;

var hubUrl = "http://localhost:5192/hubs/chat";

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

// Register the handler before connecting.
connection.On<string>("ReceiveMessage", message =>
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
                case "join" when parts.Length >= 2:
                    await connection.InvokeAsync(
                        "JoinGroupChat", parts[1]);
                    Console.WriteLine("Join requested.");
                    break;

                case "send" when parts.Length == 3:
                    await connection.InvokeAsync(
                        "SendMessage", parts[1], parts[2]);
                    Console.WriteLine("Send requested.");
                    break;

                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hub invocation failed: {ex.Message}");
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
