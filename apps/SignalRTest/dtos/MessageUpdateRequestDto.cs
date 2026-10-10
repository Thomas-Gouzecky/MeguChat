using System.Text.Json.Serialization;

public class MessageUpdateRequestDto
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}