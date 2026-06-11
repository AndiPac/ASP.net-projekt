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

public class PatientAttachmentApiControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const int SeedClinicId = 9101;
    private const int SeedOwnerId = 9102;
    private const int SeedPatientId = 9103;

    public PatientAttachmentApiControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task UserRole_CanViewAttachments_ButCannotUploadOrDelete()
    {
        using var client = CreateClientForRole("User");

        var listResponse = await client.GetAsync($"/api/patients/{SeedPatientId}/attachments");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        using var uploadContent = BuildUploadContent();
        var uploadResponse = await client.PostAsync($"/api/patients/{SeedPatientId}/attachments", uploadContent);
        Assert.Equal(HttpStatusCode.Forbidden, uploadResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{SeedPatientId}/attachments/1");
        Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
    }

    [Theory]
    [InlineData("Vet")]
    [InlineData("Administrator")]
    public async Task VetAndAdministrator_CanUploadAndDeleteAttachments(string role)
    {
        using var client = CreateClientForRole(role);

        using var uploadContent = BuildUploadContent();
        var uploadResponse = await client.PostAsync($"/api/patients/{SeedPatientId}/attachments", uploadContent);
        Assert.Equal(HttpStatusCode.OK, uploadResponse.StatusCode);

        var payload = await uploadResponse.Content.ReadFromJsonAsync<UploadResponse>();
        Assert.NotNull(payload);
        Assert.True(payload!.id > 0);

        var deleteResponse = await client.DeleteAsync($"/api/patients/{SeedPatientId}/attachments/{payload.id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    private HttpClient CreateClientForRole(string role)
    {
        var databaseName = $"VetAmbAttachmentTests_{Guid.NewGuid():N}";

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
                        options.DefaultAuthenticateScheme = RoleBasedTestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = RoleBasedTestAuthHandler.SchemeName;
                        options.DefaultScheme = RoleBasedTestAuthHandler.SchemeName;
                    })
                    .AddScheme<AuthenticationSchemeOptions, RoleBasedTestAuthHandler>(RoleBasedTestAuthHandler.SchemeName, _ => { });
            });
        });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VetAmbDbContext>();
        db.Database.EnsureCreated();
        SeedEntities(db);

        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Role", role);

        return client;
    }

    private static MultipartFormDataContent BuildUploadContent()
    {
        var content = new MultipartFormDataContent();
        var bytes = new byte[] { 1, 2, 3, 4 };
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
        content.Add(fileContent, "file", "test.txt");
        return content;
    }

    private static void SeedEntities(VetAmbDbContext db)
    {
        if (!db.Clinics.Any(c => c.Id == SeedClinicId))
        {
            db.Clinics.Add(new Clinic
            {
                Id = SeedClinicId,
                Name = "Seed Clinic Attachments",
                Address = "Seed Street 1",
                Phone = "01 444 4444",
                Email = "seed.attach@clinic.com",
                FoundationDate = new DateTime(2020, 5, 5),
                MaxCapacity = 42,
                RegistrationNumber = "SEED-ATT-CLN"
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
                Email = "seed.owner.attach@vetamb.com",
                Phone = "091 444 4444",
                Address = "Seed Owner Address",
                RegistrationDate = DateTime.UtcNow,
                IdNumber = "OWN-SEED-ATT",
                ClinicId = SeedClinicId
            });
            db.SaveChanges();
        }

        if (!db.Patients.Any(p => p.Id == SeedPatientId))
        {
            db.Patients.Add(new Patient
            {
                Id = SeedPatientId,
                Name = "Seed Patient Attachments",
                Species = AnimalSpecies.Dog,
                Breed = "Beagle",
                DateOfBirth = new DateTime(2021, 1, 1),
                Weight = 12.3m,
                MicrochipId = "MC-SEED-ATT",
                Color = "Brown",
                OwnerId = SeedOwnerId
            });
            db.SaveChanges();
        }
    }

    private sealed class UploadResponse
    {
        public int id { get; set; }
    }

    private sealed class RoleBasedTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "RoleBasedTestAuth";

        public RoleBasedTestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var role = Request.Headers.TryGetValue("X-Test-Role", out var roleHeader)
                ? roleHeader.ToString()
                : "User";

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
                new Claim(ClaimTypes.Name, "integration-test-user"),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
