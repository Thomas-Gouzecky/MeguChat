using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/ai")]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] AIMessageRequest request)
    {
        var result = await _aiService.ChatAsync(request);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        var isHealthy = _aiService.IsHealthy();
        if (isHealthy)
        {
            return Ok("AI Service is running.");
        }
        return StatusCode(503, "AI Service is not healthy.");
    }
}