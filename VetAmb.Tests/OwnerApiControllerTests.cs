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
using VetAmb.Models;
using Xunit;

namespace VetAmb.Tests;

public class OwnerApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const int SeedClinicId = 5201;

    public OwnerApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/owners");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var owners = await getResponse.Content.ReadFromJsonAsync<List<OwnerResponseDto>>();
        Assert.NotNull(owners);

        var payload = new
        {
            FirstName = "Test",
            LastName = "Owner",
            Email = "test.owner@vetamb.com",
            Phone = "091 222 3333",
            Address = "Owner Street 1",
            IdNumber = "OWN-INT-001",
            ClinicId = SeedClinicId
        };

        var postResponse = await client.PostAsJsonAsync("/api/owners", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<OwnerResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(payload.FirstName, created.FirstName);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string firstName = "Flow";
        const string lastName = "Owner";

        var createPayload = new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = "flow.owner@vetamb.com",
            Phone = "091 444 5555",
            Address = "Flow Street 2",
            IdNumber = "OWN-INT-002",
            ClinicId = SeedClinicId
        };

        var createResponse = await client.PostAsJsonAsync("/api/owners", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/owners");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var owners = await allResponse.Content.ReadFromJsonAsync<List<OwnerResponseDto>>();
        Assert.NotNull(owners);

        var created = owners!.OrderByDescending(o => o.Id)
            .FirstOrDefault(o => o.FirstName == firstName && o.LastName == lastName);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/owners/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putPayload = new
        {
            Id = created.Id,
            FirstName = "Updated",
            LastName = "Owner",
            Email = "updated.owner@vetamb.com",
            Phone = "091 666 7777",
            Address = "Updated Street 3",
            IdNumber = "OWN-INT-003",
            ClinicId = SeedClinicId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/owners/{created.Id}", putPayload);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/owners/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/owners/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putPayload = new
        {
            Id = missingId,
            FirstName = "Missing",
            LastName = "Owner",
            Email = "missing.owner@vetamb.com",
            Phone = "091 000 0000",
            Address = "Unknown 1",
            IdNumber = "OWN-MISSING",
            ClinicId = SeedClinicId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/owners/{missingId}", putPayload);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/owners/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            Phone = "abc",
            Address = "",
            IdNumber = "",
            ClinicId = 0
        };

        var response = await client.PostAsJsonAsync("/api/owners", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/owners", new
        {
            FirstName = "Valid",
            LastName = "Owner",
            Email = "valid.owner@vetamb.com",
            Phone = "091 111 2222",
            Address = "Valid Street",
            IdNumber = "OWN-VALID-PUT",
            ClinicId = SeedClinicId
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<OwnerResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            FirstName = "Updated",
            LastName = "Owner",
            Email = "invalid-email",
            Phone = "",
            Address = "Updated Street",
            IdNumber = "",
            ClinicId = 0
        };

        var response = await client.PutAsJsonAsync($"/api/owners/{created.Id}", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

                services.AddDbContext<VetAmbDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));

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
        if (!db.Clinics.Any(c => c.Id == SeedClinicId))
        {
            db.Clinics.Add(new Clinic
            {
                Id = SeedClinicId,
                Name = "Seed Clinic Owner",
                Address = "Owner Street 1",
                Phone = "01 200 2000",
                Email = "seed-owner@clinic.com",
                FoundationDate = new DateTime(2020, 2, 2),
                MaxCapacity = 22,
                RegistrationNumber = "SEED-OWN-CLN"
            });
            db.SaveChanges();
        }
    }

    private sealed class OwnerResponseDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
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
