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

public class MedicalRecordApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const int ClinicId = 13001;
    private const int OwnerId = 13002;
    private const int PatientId = 13003;

    public MedicalRecordApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/medical-records");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var records = await getResponse.Content.ReadFromJsonAsync<List<MedicalRecordResponseDto>>();
        Assert.NotNull(records);

        var payload = CreateValidPayload("Diag A");
        var postResponse = await client.PostAsJsonAsync("/api/medical-records", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<MedicalRecordResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string diagnosis = "Diag B";

        var createResponse = await client.PostAsJsonAsync("/api/medical-records", CreateValidPayload(diagnosis));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/medical-records");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var records = await allResponse.Content.ReadFromJsonAsync<List<MedicalRecordResponseDto>>();
        Assert.NotNull(records);

        var created = records!.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Diagnosis == diagnosis);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/medical-records/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/medical-records/{created.Id}", CreateValidPayload("Diag Updated"));
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/medical-records/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/medical-records/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/medical-records/{missingId}", CreateValidPayload("Ghost Diag"));
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/medical-records/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            RecordDate = default(DateTime),
            Diagnosis = "",
            Treatment = "",
            Notes = "Invalid",
            PatientId = 0
        };

        var response = await client.PostAsJsonAsync("/api/medical-records", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/medical-records", new
        {
            RecordDate = new DateTime(2024, 3, 15, 10, 30, 0),
            Diagnosis = "Valid Diagnosis",
            Treatment = "Valid Treatment",
            Notes = "Notes",
            PatientId = PatientId
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<MedicalRecordResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            RecordDate = default(DateTime),
            Diagnosis = "Updated Diagnosis",
            Treatment = "Updated Treatment",
            Notes = "Updated Notes",
            PatientId = PatientId
        };

        var response = await client.PutAsJsonAsync($"/api/medical-records/{created.Id}", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private object CreateValidPayload(string diagnosis)
    {
        return new
        {
            RecordDate = new DateTime(2024, 3, 15, 10, 30, 0),
            Diagnosis = diagnosis,
            Treatment = "Treatment plan",
            Notes = "Notes",
            PatientId
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
        if (!db.Clinics.Any(c => c.Id == ClinicId))
        {
            db.Clinics.Add(new Clinic
            {
                Id = ClinicId,
                Name = "Seed Clinic for MedicalRecord Tests",
                Address = "Seed Address 4",
                Phone = "01 000 0004",
                Email = "seed-medical-clinic@test.com",
                FoundationDate = new DateTime(2020, 4, 1),
                MaxCapacity = 35,
                RegistrationNumber = "SEED-MEDICAL-CLINIC"
            });
        }

        if (!db.Owners.Any(o => o.Id == OwnerId))
        {
            db.Owners.Add(new Owner
            {
                Id = OwnerId,
                FirstName = "Seed",
                LastName = "MedicalOwner",
                Email = "seed-med-owner@test.com",
                Phone = "01 444 4444",
                Address = "Owner Street 2",
                RegistrationDate = DateTime.UtcNow,
                IdNumber = "SEED-MED-OWNER",
                ClinicId = ClinicId
            });
        }

        if (!db.Patients.Any(p => p.Id == PatientId))
        {
            db.Patients.Add(new Patient
            {
                Id = PatientId,
                Name = "Seed Patient",
                Species = AnimalSpecies.Dog,
                Breed = "Breed",
                DateOfBirth = new DateTime(2020, 1, 1),
                Weight = 12.2m,
                MicrochipId = "MC-SEED-MED",
                Color = "Brown",
                OwnerId = OwnerId
            });
        }

        db.SaveChanges();
    }

    private sealed class MedicalRecordResponseDto
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
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
