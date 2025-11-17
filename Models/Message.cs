using System.Text.Json.Serialization;

namespace pcinx_api.Models;

public record Message
{
    [JsonPropertyName("level")] 
    public string Level { get; init; } = "info"; // info|warning|error|ok
    
    [JsonPropertyName("text")] 
    public string Text { get; init; } = "";
}

