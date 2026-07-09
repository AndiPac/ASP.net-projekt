using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VetAmb.McpTools;

public sealed class McpAccessOptions
{
    public bool Enabled { get; set; } = true;
    public bool AllowInProduction { get; set; } = false;
    public bool ReadOnlyByDefault { get; set; } = true;
    public bool AllowWriteOperations { get; set; } = false;
    public bool RequireAuthenticatedUserForWrite { get; set; } = false;
    public string[] WriteRoles { get; set; } = Array.Empty<string>();
}

public sealed class McpToolExecution
{
    private readonly IOptions<McpAccessOptions> _options;
    private readonly IHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<McpToolExecution> _logger;

    public McpToolExecution(
        IOptions<McpAccessOptions> options,
        IHostEnvironment environment,
        IHttpContextAccessor httpContextAccessor,
        ILogger<McpToolExecution> logger)
    {
        _options = options;
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public TResult ExecuteRead<TResult>(string toolName, Func<TResult> action)
    {
        return Execute(toolName, isWriteOperation: false, action);
    }

    public TResult ExecuteWrite<TResult>(string toolName, Func<TResult> action)
    {
        return Execute(toolName, isWriteOperation: true, action);
    }

    private TResult Execute<TResult>(string toolName, bool isWriteOperation, Func<TResult> action)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
        var mode = isWriteOperation ? "Write" : "Read";

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["RequestId"] = correlationId,
            ["UserId"] = userId,
            ["Route"] = $"mcp:{toolName}",
            ["ToolName"] = toolName,
            ["ToolMode"] = mode
        });

        _logger.LogInformation("MCP tool invocation started. Tool: {ToolName}, Mode: {ToolMode}", toolName, mode);

        try
        {
            EnsureAccess(toolName, isWriteOperation, user);
            var result = action();
            _logger.LogInformation("MCP tool invocation completed. Tool: {ToolName}, Outcome: {Outcome}", toolName, "Success");
            return result;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "MCP tool access denied. Tool: {ToolName}, Outcome: {Outcome}", toolName, "Forbidden");
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "MCP tool validation failed. Tool: {ToolName}, Outcome: {Outcome}", toolName, "ValidationFailed");
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "MCP tool operation failed. Tool: {ToolName}, Outcome: {Outcome}", toolName, "InvalidOperation");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MCP tool invocation failed. Tool: {ToolName}, Outcome: {Outcome}", toolName, "UnhandledError");
            throw;
        }
    }

    private void EnsureAccess(string toolName, bool isWriteOperation, ClaimsPrincipal? user)
    {
        var options = _options.Value;

        if (!options.Enabled)
        {
            throw new UnauthorizedAccessException("MCP tools are disabled by configuration.");
        }

        if (_environment.IsProduction() && !options.AllowInProduction)
        {
            throw new UnauthorizedAccessException("MCP tools are disabled in Production.");
        }

        if (!isWriteOperation)
        {
            return;
        }

        if (options.ReadOnlyByDefault && !options.AllowWriteOperations)
        {
            throw new UnauthorizedAccessException("MCP is running in read-only mode. Write operations are disabled.");
        }

        if (options.RequireAuthenticatedUserForWrite && (user?.Identity?.IsAuthenticated != true))
        {
            throw new UnauthorizedAccessException("Authenticated user is required for MCP write operations.");
        }

        if (options.WriteRoles is { Length: > 0 })
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException($"MCP write operation '{toolName}' requires an authenticated user with one of the configured roles.");
            }

            var hasRequiredRole = options.WriteRoles.Any(user.IsInRole);
            if (!hasRequiredRole)
            {
                throw new UnauthorizedAccessException($"MCP write operation '{toolName}' is not allowed for the current user role set.");
            }
        }
    }
}
