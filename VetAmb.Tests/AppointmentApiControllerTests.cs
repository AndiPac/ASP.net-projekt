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

public class AppointmentApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const int ClinicId = 14001;
    private const int OwnerId = 14002;
    private const int PatientId = 14003;
    private const int VetId = 14004;
    private const int ServiceId = 14005;

    public AppointmentApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CrudReadWrite_ShouldReturn200ForGetAll_And201ForCreate()
    {
        using var client = CreateAuthenticatedClient();

        var getResponse = await client.GetAsync("/api/appointments");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var appointments = await getResponse.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>();
        Assert.NotNull(appointments);

        var payload = CreateValidPayload("Appointment A");
        var postResponse = await client.PostAsJsonAsync("/api/appointments", payload);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var created = await postResponse.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        Assert.NotNull(created);
    }

    [Fact]
    public async Task SuccessScenarios_ShouldReturn200ForValidId_And204ForPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const string reason = "Appointment B";

        var createResponse = await client.PostAsJsonAsync("/api/appointments", CreateValidPayload(reason));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var allResponse = await client.GetAsync("/api/appointments");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);

        var appointments = await allResponse.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>();
        Assert.NotNull(appointments);

        var created = appointments!.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Reason == reason);
        Assert.NotNull(created);

        var getByIdResponse = await client.GetAsync($"/api/appointments/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getByIdResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/appointments/{created.Id}", CreateValidPayload("Appointment Updated"));
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/appointments/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task NonExistentIds_ShouldReturn404ForGetPutAndDelete()
    {
        using var client = CreateAuthenticatedClient();

        const int missingId = 999999;

        var getResponse = await client.GetAsync($"/api/appointments/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var putResponse = await client.PutAsJsonAsync($"/api/appointments/{missingId}", CreateValidPayload("Ghost Appointment"));
        Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/appointments/{missingId}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPostPayload()
    {
        using var client = CreateAuthenticatedClient();

        var invalidPayload = new
        {
            AppointmentDateTime = default(DateTime),
            Reason = "",
            Status = "",
            Notes = "Invalid",
            RescheduleReason = "",
            PatientId = 0,
            VetId = 0,
            ServiceIds = new List<int>()
        };

        var response = await client.PostAsJsonAsync("/api/appointments", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ValidationErrors_ShouldReturn400ForInvalidPutPayload()
    {
        using var client = CreateAuthenticatedClient();

        var createResponse = await client.PostAsJsonAsync("/api/appointments", new
        {
            AppointmentDateTime = new DateTime(2024, 3, 15, 11, 15, 0),
            Reason = "Valid Appointment",
            Status = nameof(AppointmentStatus.Scheduled),
            Notes = "Notes",
            RescheduleReason = "",
            PatientId = PatientId,
            VetId = VetId,
            ServiceIds = new[] { ServiceId }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<AppointmentResponseDto>();
        Assert.NotNull(created);

        var invalidPayload = new
        {
            Id = created!.Id,
            AppointmentDateTime = default(DateTime),
            Reason = "Updated Appointment",
            Status = "",
            Notes = "Updated Notes",
            RescheduleReason = "Updated",
            PatientId = PatientId,
            VetId = VetId,
            ServiceIds = Array.Empty<int>()
        };

        var response = await client.PutAsJsonAsync($"/api/appointments/{created.Id}", invalidPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private object CreateValidPayload(string reason)
    {
        return new
        {
            AppointmentDateTime = new DateTime(2024, 3, 15, 11, 15, 0),
            Reason = reason,
            Status = nameof(AppointmentStatus.Scheduled),
            Notes = "Initial notes",
            RescheduleReason = "",
            PatientId,
            VetId,
            ServiceIds = new[] { ServiceId }
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
                Name = "Seed Clinic for Appointment Tests",
                Address = "Seed Address 5",
                Phone = "01 000 0005",
                Email = "seed-appointment-clinic@test.com",
                FoundationDate = new DateTime(2020, 5, 1),
                MaxCapacity = 40,
                RegistrationNumber = "SEED-APPOINTMENT-CLINIC"
            });
        }

        if (!db.Owners.Any(o => o.Id == OwnerId))
        {
            db.Owners.Add(new Owner
            {
                Id = OwnerId,
                FirstName = "Seed",
                LastName = "AppointmentOwner",
                Email = "seed-appt-owner@test.com",
                Phone = "01 333 3333",
                Address = "Owner Street 3",
                RegistrationDate = DateTime.UtcNow,
                IdNumber = "SEED-APPT-OWNER",
                ClinicId = ClinicId
            });
        }

        if (!db.Patients.Any(p => p.Id == PatientId))
        {
            db.Patients.Add(new Patient
            {
                Id = PatientId,
                Name = "Appointment Patient",
                Species = AnimalSpecies.Dog,
                Breed = "Breed",
                DateOfBirth = new DateTime(2019, 1, 1),
                Weight = 15.4m,
                MicrochipId = "MC-SEED-APPT",
                Color = "Black",
                OwnerId = OwnerId
            });
        }

        if (!db.Vets.Any(v => v.Id == VetId))
        {
            db.Vets.Add(new Vet
            {
                Id = VetId,
                FirstName = "Seed",
                LastName = "Vet",
                Specialization = VeterinarySpecialization.GeneralPractice,
                LicenseNumber = "SEED-VET-APPT",
                YearsOfExperience = 6,
                Phone = "01 666 6666",
                HourlyRate = 70m,
                ClinicId = ClinicId
            });
        }

        if (!db.Services.Any(s => s.Id == ServiceId))
        {
            db.Services.Add(new Service
            {
                Id = ServiceId,
                Name = "Seed Service Appointment",
                Description = "Seed",
                Price = 50m,
                EstimatedDurationMinutes = 30
            });
        }

        db.SaveChanges();
    }

    private sealed class AppointmentResponseDto
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
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
