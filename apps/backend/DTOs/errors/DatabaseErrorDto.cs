using System.Text.Json.Serialization;

public class DatabaseErrorDto
{
    [JsonPropertyName("detail")]
    public string ErrorMessage { get; set; } = string.Empty;
}