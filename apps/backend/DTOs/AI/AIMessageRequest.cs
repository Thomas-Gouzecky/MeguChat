using System.Text.Json.Serialization;

public class AIMessageRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;
}