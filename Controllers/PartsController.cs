using Microsoft.AspNetCore.Mvc;
using pcinx_api.Models;
using pcinx_api.Services;

namespace pcinx_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Part>>> GetParts([FromQuery] string? category)
    {
        var parts = await _partService.GetPartsByCategoryAsync(category);
        return Ok(parts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Part>> GetPart(string id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        
        if (part == null)
            return NotFound();
        
        return Ok(part);
    }
}

