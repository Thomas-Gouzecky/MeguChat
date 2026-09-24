using System.Text.Json.Serialization;

public class MessageUpdateRequestDto
{
    [JsonPropertyName("content")]
    public string Message { get; set; } = string.Empty;
}