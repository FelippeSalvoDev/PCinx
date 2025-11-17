using System.Text.Json;
using pcinx_api.Models;

namespace pcinx_api.Services;

public class BuildService : IBuildService
{
    private readonly string _buildsDirectory;

    public BuildService()
    {
        _buildsDirectory = Path.Combine(AppContext.BaseDirectory, "Data", "builds");
        Directory.CreateDirectory(_buildsDirectory);
    }

    public Task<string> SaveBuildAsync(List<Part> parts)
    {
        var code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var build = new Build
        {
            Code = code,
            CreatedAt = DateTimeOffset.UtcNow,
            Parts = parts
        };

        var filePath = Path.Combine(_buildsDirectory, $"{code}.json");
        var json = JsonSerializer.Serialize(build, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);

        return Task.FromResult(code);
    }

    public Task<Build?> GetBuildByCodeAsync(string code)
    {
        var filePath = Path.Combine(_buildsDirectory, $"{code}.json");
        
        if (!File.Exists(filePath))
            return Task.FromResult<Build?>(null);

        var json = File.ReadAllText(filePath);
        var build = JsonSerializer.Deserialize<Build>(json);
        
        return Task.FromResult(build);
    }
}

