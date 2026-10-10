using System.Text.Json.Serialization;

public class MessageCreationRequestDto
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}