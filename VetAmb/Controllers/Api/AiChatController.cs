using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Post([FromBody] List<ChatMessage>? messages, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _chatbotService.GetResponseAsync(messages ?? new List<ChatMessage>(), User, cancellationToken);
            return Ok(new { response });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "AI chat configuration error.");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "AI chat is not configured. Please set the OpenAI API key."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing AI chat request.");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Chat assistant is temporarily unavailable."
            });
        }
    }
}
