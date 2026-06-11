using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VetAmb.Data;
using Xunit;

namespace VetAmb.Tests;

public class ServiceApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ServiceApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/services");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var services = await getResponse.Content.ReadFromJsonAsync<List<ServiceResponseDto>>();
        Assert.NotNull(services);

        var payload = CreateValidPayload("Service A");
        var postResponse = await client.PostAsJsonAsync("/api/services", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<ServiceResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("Service A", created.Name);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string serviceName = "Service B";

        var createResponse = await client.PostAsJsonAsync("/api/services", CreateValidPayload(serviceName));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/services");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var services = await allResponse.Content.ReadFromJsonAsync<List<ServiceResponseDto>>();
        Assert.NotNull(services);

        var created = services!.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Name == serviceName);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/services/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/services/{created.Id}", CreateValidPayload("Service Updated"));
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/services/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/services/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/services/{missingId}", CreateValidPayload("Ghost Service"));
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/services/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            Name = "",
            Description = "Invalid service",
            Price = 10m,
            EstimatedDurationMinutes = 0
        };

        var response = await client.PostAsJsonAsync("/api/services", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/services", new
        {
            Name = "Valid Service",
            Description = "Valid",
            Price = 55m,
            EstimatedDurationMinutes = 25
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ServiceResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            Name = "Updated Service",
            Description = "Updated",
            Price = 0m,
            EstimatedDurationMinutes = 0
        };

        var response = await client.PutAsJsonAsync($"/api/services/{created.Id}", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static object CreateValidPayload(string name)
    {
        return new
        {
            Name = name,
            Description = "Test description",
            Price = 45.5m,
            EstimatedDurationMinutes = 30
        };
    }

    private HttpClient CreateAuthenticatedClient()
    {
        var databaseName = $"VetAmbTests_{Guid.NewGuid()}";

        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<VetAmbDbContext>));
                services.RemoveAll(typeof(VetAmbDbContext));

                services.AddDbContext<VetAmbDbContext>(options => options.UseInMemoryDatabase(databaseName));

                services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                        options.DefaultScheme = TestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
            });
        });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VetAmbDbContext>();
        db.Database.EnsureCreated();
        SeedEntities(db);

        return factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    private static void SeedEntities(VetAmbDbContext db)
    {
        if (!db.Services.Any())
        {
            db.Services.Add(new VetAmb.Models.Service
            {
                Name = "Seed Service",
                Description = "Seed",
                Price = 20m,
                EstimatedDurationMinutes = 20
            });
        }

        db.SaveChanges();
    }

    private sealed class ServiceResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestAuth";

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
                new Claim(ClaimTypes.Name, "integration-test-user"),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
