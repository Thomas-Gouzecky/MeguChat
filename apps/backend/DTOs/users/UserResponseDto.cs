using System.Text.Json.Serialization;

public class UserResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}