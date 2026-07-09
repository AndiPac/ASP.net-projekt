using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace VetAmb.Tests;

public class AiChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AiChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_WithMissingOpenAiConfiguration_ReturnsServerError()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["OpenAI:ApiKey"] = "PLACEHOLDER_TEST_KEY",
                    ["OpenAI:Model"] = "gpt-4o-mini"
                });
            });
        });

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.PostAsJsonAsync("/api/chat", new[]
        {
            new { role = "user", content = "Hello" }
        });

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Post_ShouldBeRateLimited_WhenRequestsExceedConfiguredLimit()
    {
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["OpenAI:ApiKey"] = "PLACEHOLDER_TEST_KEY",
                    ["OpenAI:Model"] = "gpt-4o-mini",
                    ["OpenAI:ChatRateLimit:PermitLimit"] = "2",
                    ["OpenAI:ChatRateLimit:WindowMinutes"] = "1"
                });
            });
        });

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var payload = new[] { new { role = "user", content = "Hello" } };

        var first = await client.PostAsJsonAsync("/api/chat", payload);
        var second = await client.PostAsJsonAsync("/api/chat", payload);
        var third = await client.PostAsJsonAsync("/api/chat", payload);

        Assert.Equal(HttpStatusCode.InternalServerError, first.StatusCode);
        Assert.Equal(HttpStatusCode.InternalServerError, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
    }
}
