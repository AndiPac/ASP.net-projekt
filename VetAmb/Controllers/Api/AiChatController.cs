using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using VetAmb.Models;
using VetAmb.Services;

namespace VetAmb.Controllers.Api;

[ApiController]
[Route("api/chat")]
public class AiChatController : ControllerBase
{
    private readonly AiChatbotService _chatbotService;
    private readonly ILogger<AiChatController> _logger;

    public AiChatController(AiChatbotService chatbotService, ILogger<AiChatController> logger)
    {
        _chatbotService = chatbotService;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    [EnableRateLimiting("AiChatPolicy")]
    public async Task<IActionResult> Post([FromBody] List<ChatMessage>? messages, CancellationToken cancellationToken)
    {
        var requestId = HttpContext.TraceIdentifier;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var route = HttpContext.GetEndpoint()?.DisplayName ?? HttpContext.Request.Path.Value ?? string.Empty;
        var messageCount = messages?.Count ?? 0;

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["RequestId"] = requestId,
            ["UserId"] = userId,
            ["Route"] = route
        });

        _logger.LogInformation(
            "AI chat request started. MessageCount: {MessageCount}, IsAuthenticated: {IsAuthenticated}",
            messageCount,
            User.Identity?.IsAuthenticated == true);

        try
        {
            var response = await _chatbotService.GetResponseAsync(messages ?? new List<ChatMessage>(), User, cancellationToken);
            _logger.LogInformation("AI chat request completed successfully. Outcome: {Outcome}", "Success");
            return Ok(new { response });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "AI chat configuration issue. Outcome: {Outcome}", "ConfigurationError");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "AI chat is not configured. Please set the OpenAI API key."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing AI chat request. Outcome: {Outcome}", "UnhandledError");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Chat assistant is temporarily unavailable."
            });
        }
    }
}
