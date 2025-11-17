namespace pcinx_api.Models;

public record BuildRequest
{
    public List<string> PartIds { get; init; } = new();
}

