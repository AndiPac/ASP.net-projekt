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

public class ClinicApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ClinicApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/clinics");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var clinics = await getResponse.Content.ReadFromJsonAsync<List<ClinicResponseDto>>();
        Assert.NotNull(clinics);

        var payload = new
        {
            Name = "New API Clinic",
            Address = "River 22",
            Phone = "01 765 4321",
            Email = "api@clinic.com",
            RegistrationNumber = "REG-API-22",
            MaxCapacity = 30,
            FoundationDate = new DateTime(2022, 3, 15)
        };

        var postResponse = await client.PostAsJsonAsync("/api/clinics", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal(payload.Name, created.Name);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string clinicName = "Clinic For Update";

        var createPayload = new
        {
            Name = clinicName,
            Address = "Updated Street 1",
            Phone = "01 111 2222",
            Email = "update@clinic.com",
            RegistrationNumber = "REG-UPD-1",
            MaxCapacity = 15,
            FoundationDate = new DateTime(2021, 1, 1)
        };

        var createResponse = await client.PostAsJsonAsync("/api/clinics", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

    var allClinicsResponse = await client.GetAsync("/api/clinics");
    Assert.Equal(HttpStatusCode.OK, allClinicsResponse.StatusCode);

    var clinics = await allClinicsResponse.Content.ReadFromJsonAsync<List<ClinicResponseDto>>();
    Assert.NotNull(clinics);

    var created = clinics!.OrderByDescending(c => c.Id).FirstOrDefault(c => c.Name == clinicName);
    Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/clinics/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putPayload = new
        {
            Id = created.Id,
            Name = "Clinic Updated Name",
            Address = "Updated Street 2",
            Phone = "01 333 4444",
            Email = "updated@clinic.com",
            RegistrationNumber = "REG-UPD-2",
            MaxCapacity = 20,
            FoundationDate = new DateTime(2021, 1, 1)
        };

        var putResponse = await client.PutAsJsonAsync($"/api/clinics/{created.Id}", putPayload);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/clinics/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/clinics/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putPayload = new
        {
            Id = missingId,
            Name = "Missing Clinic",
            Address = "Unknown 1",
            Phone = "01 999 0000",
            Email = "missing@clinic.com",
            RegistrationNumber = "REG-MISS",
            MaxCapacity = 11,
            FoundationDate = new DateTime(2021, 1, 1)
        };

        var putResponse = await client.PutAsJsonAsync($"/api/clinics/{missingId}", putPayload);
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/clinics/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Post_ShouldCreateClinicAndReturn201_WithValidPayload()
    {
        using var client = CreateAuthenticatedClient();

        var payload = new
        {
            Name = "POST Dedicated Clinic",
            Address = "POST Street 1",
            Phone = "01 100 1001",
            Email = "post@clinic.com",
            RegistrationNumber = "REG-POST-01",
            MaxCapacity = 12,
            FoundationDate = new DateTime(2023, 6, 10)
        };

        var response = await client.PostAsJsonAsync("/api/clinics", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(body);
        Assert.True(body!.Id > 0);
        Assert.Equal(payload.Name, body.Name);
        Assert.Equal(payload.Address, body.Address);
        Assert.Equal(payload.RegistrationNumber, body.RegistrationNumber);
        Assert.Equal(payload.MaxCapacity, body.MaxCapacity);
    }

    [Fact]
    public async Task Put_ShouldUpdateExistingClinicAndReturn204()
    {
        using var client = CreateAuthenticatedClient();

        // Create a clinic to update
        var createPayload = new
        {
            Name = "PUT Before Clinic",
            Address = "PUT Street 1",
            Phone = "01 200 2001",
            Email = "put.before@clinic.com",
            RegistrationNumber = "REG-PUT-01",
            MaxCapacity = 8,
            FoundationDate = new DateTime(2022, 1, 1)
        };

        var createResponse = await client.PostAsJsonAsync("/api/clinics", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(created);

        // Update it
        var updatePayload = new
        {
            Id = created!.Id,
            Name = "PUT After Clinic",
            Address = "PUT Street 2",
            Phone = "01 200 2002",
            Email = "put.after@clinic.com",
            RegistrationNumber = "REG-PUT-02",
            MaxCapacity = 16,
            FoundationDate = new DateTime(2022, 1, 1)
        };

        var putResponse = await client.PutAsJsonAsync($"/api/clinics/{created.Id}", updatePayload);
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        // Verify the change persisted
        var getResponse = await client.GetAsync($"/api/clinics/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var updated = await getResponse.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(updated);
        Assert.Equal("PUT After Clinic", updated!.Name);
        Assert.Equal("PUT Street 2", updated.Address);
        Assert.Equal(16, updated.MaxCapacity);
    }

    [Fact]
    public async Task Delete_ShouldSoftDeleteExistingClinicAndReturn204()
    {
        using var client = CreateAuthenticatedClient();

        // Create a clinic to delete
        var createPayload = new
        {
            Name = "DELETE Target Clinic",
            Address = "DELETE Street 1",
            Phone = "01 300 3001",
            Email = "delete@clinic.com",
            RegistrationNumber = "REG-DEL-01",
            MaxCapacity = 5,
            FoundationDate = new DateTime(2021, 5, 20)
        };

        var createResponse = await client.PostAsJsonAsync("/api/clinics", createPayload);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(created);

        // Delete it
        var deleteResponse = await client.DeleteAsync($"/api/clinics/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify it is no longer accessible
        var getResponse = await client.GetAsync($"/api/clinics/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            Name = "",
            Address = "Invalid Address",
            Phone = "01 555 6666",
            Email = "invalid@clinic.com",
            RegistrationNumber = "REG-INV",
            MaxCapacity = 25,
            FoundationDate = default(DateTime)
        };

        var response = await client.PostAsJsonAsync("/api/clinics", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/clinics", new
        {
            Name = "Clinic For Invalid Put",
            Address = "Street 1",
            Phone = "01 111 1111",
            Email = "valid@clinic.com",
            RegistrationNumber = "REG-VALID",
            MaxCapacity = 10,
            FoundationDate = new DateTime(2022, 1, 1)
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<ClinicResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            Name = "Updated Clinic",
            Address = "Updated Street",
            Phone = "01 222 2222",
            Email = "updated@clinic.com",
            RegistrationNumber = "REG-UPD-INVALID",
            MaxCapacity = 12,
            FoundationDate = default(DateTime)
        };

        var response = await client.PutAsJsonAsync($"/api/clinics/{created.Id}", invalidPayload);
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
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        TestAuthHandler.SchemeName,
                        _ => { });
            });
        });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VetAmbDbContext>();
        db.Database.EnsureCreated();
        SeedClinics(db);

        return factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    private static void SeedClinics(VetAmbDbContext db)
    {
        if (db.Clinics.Any())
        {
            return;
        }

        db.Clinics.Add(new Clinic
        {
            Name = "Test Clinic",
            Address = "Main Street 1",
            Phone = "01 123 4567",
            Email = "test@clinic.com",
            FoundationDate = new DateTime(2020, 1, 1),
            MaxCapacity = 10,
            RegistrationNumber = "REG-001"
        });

        db.SaveChanges();
    }

    private sealed class ClinicResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime FoundationDate { get; set; }
        public int MaxCapacity { get; set; }
        public string? RegistrationNumber { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "TestAuth";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
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
