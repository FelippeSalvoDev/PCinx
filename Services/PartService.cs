using System.Text.Json;
using pcinx_api.Models;

namespace pcinx_api.Services;

public class PartService : IPartService
{
    private readonly List<Part> _parts;
    private readonly string _dataPath;

    public PartService()
    {
        _dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "parts.json");
        var json = File.ReadAllTextAsync(_dataPath).GetAwaiter().GetResult();
        _parts = JsonSerializer.Deserialize<List<Part>>(json) ?? new();
    }

    public Task<List<Part>> GetAllPartsAsync()
    {
        return Task.FromResult(_parts);
    }

    public Task<List<Part>> GetPartsByCategoryAsync(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return GetAllPartsAsync();

        var filtered = _parts
            .Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        return Task.FromResult(filtered);
    }

    public Task<Part?> GetPartByIdAsync(string id)
    {
        var part = _parts.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(part);
    }
}

