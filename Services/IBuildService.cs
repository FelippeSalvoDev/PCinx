using pcinx_api.Models;

namespace pcinx_api.Services;

public interface IBuildService
{
    Task<string> SaveBuildAsync(List<Part> parts);
    Task<Build?> GetBuildByCodeAsync(string code);
}

