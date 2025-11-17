namespace pcinx_api.Models;

public record Part
{
    public string Id { get; init; } = default!;
    public string Category { get; init; } = default!; // CPU, Motherboard, GPU, RAM, Storage, PSU, Case
    public string Name { get; init; } = default!;
    public string? Brand { get; init; }
    public decimal Price { get; init; }
    public Dictionary<string, string>? Attributes { get; init; }
}

