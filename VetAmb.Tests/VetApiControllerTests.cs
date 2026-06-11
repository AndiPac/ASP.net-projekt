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

public class VetApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const int SeedClinicId = 5101;

    public VetApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/vets");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var vets = await getResponse.Content.ReadFromJsonAsync<List<VetResponseDto>>();
        Assert.NotNull(vets);

        var payload = new
        {
            FirstName = "Test",
            LastName = "Vet",
            Specialization = nameof(VeterinarySpecialization.GeneralPractice),
            LicenseNumber = "VET-INT-001",
            YearsOfExperience = 6,
            Phone = "091 111 2222",
            HourlyRate = 75.5m,
            ClinicId = SeedClinicId
        };

        var postResponse = await client.PostAsJsonAsync("/api/vets", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<VetResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(payload.FirstName, created.FirstName);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string firstName = "Flow";
        const string lastName = "Vet";

        var createPayload = new
        {
            FirstName = firstName,
            LastName = lastName,
            Specialization = nameof(VeterinarySpecialization.Surgery),
            LicenseNumber = "VET-INT-002",
            YearsOfExperience = 8,
            Phone = "092 333 4444",
            HourlyRate = 88m,
            ClinicId = SeedClinicId
        };

        var createResponse = await client.PostAsJsonAsync("/api/vets", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/vets");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var vets = await allResponse.Content.ReadFromJsonAsync<List<VetResponseDto>>();
        Assert.NotNull(vets);

        var created = vets!.OrderByDescending(v => v.Id)
            .FirstOrDefault(v => v.FirstName == firstName && v.LastName == lastName);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/vets/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putPayload = new
        {
            Id = created.Id,
            FirstName = "Updated",
            LastName = "Vet",
            Specialization = nameof(VeterinarySpecialization.Cardiology),
            LicenseNumber = "VET-INT-003",
            YearsOfExperience = 10,
            Phone = "093 555 6666",
            HourlyRate = 99m,
            ClinicId = SeedClinicId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/vets/{created.Id}", putPayload);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/vets/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/vets/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putPayload = new
        {
            Id = missingId,
            FirstName = "Missing",
            LastName = "Vet",
            Specialization = nameof(VeterinarySpecialization.Dermatology),
            LicenseNumber = "VET-MISSING",
            YearsOfExperience = 2,
            Phone = "091 000 0000",
            HourlyRate = 60m,
            ClinicId = SeedClinicId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/vets/{missingId}", putPayload);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/vets/{missingId}");
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
            Specialization = "",
            LicenseNumber = "",
            YearsOfExperience = 1,
            Phone = "091 111 2222",
            HourlyRate = 50m,
            ClinicId = 0
        };

        var response = await client.PostAsJsonAsync("/api/vets", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/vets", new
        {
            FirstName = "Valid",
            LastName = "Vet",
            Specialization = nameof(VeterinarySpecialization.GeneralPractice),
            LicenseNumber = "VET-VALID-PUT",
            YearsOfExperience = 4,
            Phone = "091 222 3333",
            HourlyRate = 70m,
            ClinicId = SeedClinicId
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<VetResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            FirstName = "Updated",
            LastName = "Vet",
            Specialization = "",
            LicenseNumber = "VET-UPD-INVALID",
            YearsOfExperience = 5,
            Phone = "091 444 5555",
            HourlyRate = 80m,
            ClinicId = SeedClinicId
        };

        var response = await client.PutAsJsonAsync($"/api/vets/{created.Id}", invalidPayload);
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
                Name = "Seed Clinic Vet",
                Address = "Vet Street 1",
                Phone = "01 100 1000",
                Email = "seed-vet@clinic.com",
                FoundationDate = new DateTime(2020, 1, 1),
                MaxCapacity = 20,
                RegistrationNumber = "SEED-VET-CLN"
            });
            db.SaveChanges();
        }
    }

    private sealed class VetResponseDto
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
