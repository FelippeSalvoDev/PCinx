using pcinx_api.Models;

namespace pcinx_api.Services;

public interface IPartService
{
    Task<List<Part>> GetAllPartsAsync();
    Task<List<Part>> GetPartsByCategoryAsync(string? category);
    Task<Part?> GetPartByIdAsync(string id);
}

