using System.Text.Json.Serialization;

public class MessageCreationRequestDto
{
    [JsonPropertyName("content")]
    public string Message { get; set; } = string.Empty;
}