namespace pcinx_api.Models;

public record Build
{
    public string Code { get; init; } = default!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<Part> Parts { get; init; } = new();
}

