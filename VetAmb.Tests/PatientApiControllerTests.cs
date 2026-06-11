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

public class PatientApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const int SeedClinicId = 5301;
    private const int SeedOwnerId = 5302;

    public PatientApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var patients = await getResponse.Content.ReadFromJsonAsync<List<PatientResponseDto>>();
        Assert.NotNull(patients);

        var payload = new
        {
            Name = "TestPatient",
            Species = nameof(AnimalSpecies.Dog),
            Breed = "Beagle",
            DateOfBirth = new DateTime(2021, 1, 1),
            Weight = 12.5m,
            MicrochipId = "MC-INT-001",
            Color = "Brown",
            OwnerId = SeedOwnerId
        };

        var postResponse = await client.PostAsJsonAsync("/api/patients", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<PatientResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(payload.Name, created.Name);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string patientName = "FlowPatient";

        var createPayload = new
        {
            Name = patientName,
            Species = nameof(AnimalSpecies.Cat),
            Breed = "Siamese",
            DateOfBirth = new DateTime(2020, 2, 15),
            Weight = 4.2m,
            MicrochipId = "MC-INT-002",
            Color = "Cream",
            OwnerId = SeedOwnerId
        };

        var createResponse = await client.PostAsJsonAsync("/api/patients", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/patients");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var patients = await allResponse.Content.ReadFromJsonAsync<List<PatientResponseDto>>();
        Assert.NotNull(patients);

        var created = patients!.OrderByDescending(p => p.Id).FirstOrDefault(p => p.Name == patientName);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/patients/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putPayload = new
        {
            Id = created.Id,
            Name = "UpdatedPatient",
            Species = nameof(AnimalSpecies.Rabbit),
            Breed = "Mini Lop",
            DateOfBirth = new DateTime(2019, 10, 10),
            Weight = 2.8m,
            MicrochipId = "MC-INT-003",
            Color = "White",
            OwnerId = SeedOwnerId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/patients/{created.Id}", putPayload);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/patients/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putPayload = new
        {
            Id = missingId,
            Name = "MissingPatient",
            Species = nameof(AnimalSpecies.Bird),
            Breed = "Canary",
            DateOfBirth = new DateTime(2020, 1, 1),
            Weight = 0.2m,
            MicrochipId = "MC-MISS",
            Color = "Yellow",
            OwnerId = SeedOwnerId
        };

        var putResponse = await client.PutAsJsonAsync($"/api/patients/{missingId}", putPayload);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            Name = "",
            Species = "",
            Breed = "",
            DateOfBirth = default(DateTime),
            Weight = 0m,
            MicrochipId = "",
            Color = "",
            OwnerId = 0
        };

        var response = await client.PostAsJsonAsync("/api/patients", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/patients", new
        {
            Name = "Patient For Invalid Put",
            Species = nameof(AnimalSpecies.Dog),
            Breed = "Breed",
            DateOfBirth = new DateTime(2021, 1, 1),
            Weight = 10.2m,
            MicrochipId = "MC-VALID-PUT",
            Color = "Brown",
            OwnerId = SeedOwnerId
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<PatientResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            Name = "Updated Patient",
            Species = "",
            Breed = "Updated Breed",
            DateOfBirth = default(DateTime),
            Weight = 11.1m,
            MicrochipId = "MC-UPD-INVALID",
            Color = "Black",
            OwnerId = SeedOwnerId
        };

        var response = await client.PutAsJsonAsync($"/api/patients/{created.Id}", invalidPayload);
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
                Name = "Seed Clinic Patient",
                Address = "Patient Street 1",
                Phone = "01 300 3000",
                Email = "seed-patient@clinic.com",
                FoundationDate = new DateTime(2020, 3, 3),
                MaxCapacity = 23,
                RegistrationNumber = "SEED-PAT-CLN"
            });
            db.SaveChanges();
        }

        if (!db.Owners.Any(o => o.Id == SeedOwnerId))
        {
            db.Owners.Add(new Owner
            {
                Id = SeedOwnerId,
                FirstName = "Seed",
                LastName = "Owner",
                Email = "seed.owner@vetamb.com",
                Phone = "091 303 3030",
                Address = "Owner Address",
                RegistrationDate = DateTime.UtcNow,
                IdNumber = "OWN-SEED-PAT",
                ClinicId = SeedClinicId
            });
            db.SaveChanges();
        }
    }

    private sealed class PatientResponseDto
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
