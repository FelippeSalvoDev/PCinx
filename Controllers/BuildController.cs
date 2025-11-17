using Microsoft.AspNetCore.Mvc;
using pcinx_api.Models;
using pcinx_api.Services;

namespace pcinx_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuildController : ControllerBase
{
    private readonly IPartService _partService;
    private readonly ICompatibilityService _compatibilityService;
    private readonly IBuildService _buildService;

    public BuildController(
        IPartService partService,
        ICompatibilityService compatibilityService,
        IBuildService buildService)
    {
        _partService = partService;
        _compatibilityService = compatibilityService;
        _buildService = buildService;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<object>> ValidateBuild([FromBody] BuildRequest request)
    {
        var allParts = await _partService.GetAllPartsAsync();
        var selectedParts = allParts.Where(p => request.PartIds.Contains(p.Id)).ToList();
        
        var messages = _compatibilityService.Validate(selectedParts);
        
        return Ok(new { messages });
    }

    [HttpPost("save")]
    public async Task<ActionResult<object>> SaveBuild([FromBody] BuildRequest request)
    {
        var allParts = await _partService.GetAllPartsAsync();
        var selectedParts = allParts.Where(p => request.PartIds.Contains(p.Id)).ToList();
        
        var code = await _buildService.SaveBuildAsync(selectedParts);
        
        return Ok(new { code });
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<Build>> GetBuild(string code)
    {
        var build = await _buildService.GetBuildByCodeAsync(code);
        
        if (build == null)
            return NotFound();
        
        return Ok(build);
    }
}

