using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using VetAmb.Data;
using VetAmb.Models;
using Xunit;

namespace VetAmb.Tests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    private const int SeedClinicId = 91001;
    private const int SeedOwnerId = 91002;
    private const int SeedPatientId = 91003;
    private const int SeedVetId = 91004;
    private const int SeedServiceId = 91005;

    public ApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MainEntityGetEndpoints_ShouldReturnSuccess()
    {
        using var client = _factory.CreateClient();

        var endpoints = new[]
        {
            "/api/clinics",
            "/api/owners",
            "/api/patients",
            "/api/vets",
            "/api/services",
            "/api/medical-records",
            "/api/appointments"
        };

        foreach (var endpoint in endpoints)
        {
            var response = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task MainEntityPostEndpoints_ShouldCreateRecords()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VetAmbDbContext>();
        db.Database.EnsureCreated();
        SeedDependencies(db);

        using var client = _factory.CreateClient();

        var clinicResponse = await client.PostAsJsonAsync("/api/clinics", new
        {
            Name = "Integration Clinic",
            Address = "Integration Address 1",
            Phone = "01 555 1001",
            Email = "integration.clinic@test.com",
            FoundationDate = new DateTime(2020, 1, 1),
            MaxCapacity = 80,
            RegistrationNumber = "INT-CLINIC-001"
        });
        Assert.Equal(HttpStatusCode.Created, clinicResponse.StatusCode);

        var ownerResponse = await client.PostAsJsonAsync("/api/owners", new
        {
            FirstName = "Integration",
            LastName = "Owner",
            Email = "integration.owner@test.com",
            Phone = "091 111 2222",
            Address = "Owner Street 10",
            IdNumber = "INT-OWNER-001",
            ClinicId = SeedClinicId
        });
        Assert.Equal(HttpStatusCode.Created, ownerResponse.StatusCode);

        var patientResponse = await client.PostAsJsonAsync("/api/patients", new
        {
            Name = "Integration Patient",
            Species = nameof(AnimalSpecies.Dog),
            Breed = "Labrador",
            DateOfBirth = new DateTime(2021, 2, 2),
            Weight = 21.5m,
            MicrochipId = "INT-MC-001",
            Color = "Golden",
            OwnerId = SeedOwnerId
        });
        Assert.Equal(HttpStatusCode.Created, patientResponse.StatusCode);

        var vetResponse = await client.PostAsJsonAsync("/api/vets", new
        {
            FirstName = "Integration",
            LastName = "Vet",
            Specialization = nameof(VeterinarySpecialization.GeneralPractice),
            LicenseNumber = "INT-VET-001",
            YearsOfExperience = 8,
            Phone = "092 333 4444",
            HourlyRate = 95m,
            ClinicId = SeedClinicId
        });
        Assert.Equal(HttpStatusCode.Created, vetResponse.StatusCode);

        var serviceResponse = await client.PostAsJsonAsync("/api/services", new
        {
            Name = "Integration Service",
            Description = "Integration service description",
            Price = 49.99m,
            EstimatedDurationMinutes = 30
        });
        Assert.Equal(HttpStatusCode.Created, serviceResponse.StatusCode);

        var medicalRecordResponse = await client.PostAsJsonAsync("/api/medical-records", new
        {
            Diagnosis = "Integration diagnosis",
            Treatment = "Integration treatment",
            RecordDate = new DateTime(2026, 6, 30),
            Notes = "Integration notes",
            PatientId = SeedPatientId
        });
        Assert.Equal(HttpStatusCode.Created, medicalRecordResponse.StatusCode);

        var appointmentResponse = await client.PostAsJsonAsync("/api/appointments", new
        {
            AppointmentDateTime = new DateTime(2026, 7, 1, 9, 0, 0),
            Reason = "Integration appointment",
            Status = nameof(AppointmentStatus.Scheduled),
            Notes = "Integration appointment notes",
            RescheduleReason = string.Empty,
            PatientId = SeedPatientId,
            VetId = SeedVetId,
            ServiceIds = new[] { SeedServiceId }
        });
        Assert.Equal(HttpStatusCode.Created, appointmentResponse.StatusCode);
    }

    private static void SeedDependencies(VetAmbDbContext db)
    {
        if (!db.Clinics.Any(c => c.Id == SeedClinicId))
        {
            db.Clinics.Add(new Clinic
            {
                Id = SeedClinicId,
                Name = "Seed Clinic Integration",
                Address = "Seed Clinic Street 1",
                Phone = "01 700 7000",
                Email = "seed.clinic.integration@test.com",
                FoundationDate = new DateTime(2020, 3, 1),
                MaxCapacity = 70,
                RegistrationNumber = "SEED-CLINIC-INTEGRATION"
            });
        }

        if (!db.Owners.Any(o => o.Id == SeedOwnerId))
        {
            db.Owners.Add(new Owner
            {
                Id = SeedOwnerId,
                FirstName = "Seed",
                LastName = "OwnerIntegration",
                Email = "seed.owner.integration@test.com",
                Phone = "091 700 7000",
                Address = "Seed Owner Address",
                RegistrationDate = DateTime.UtcNow,
                IdNumber = "SEED-OWNER-INTEGRATION",
                ClinicId = SeedClinicId
            });
        }

        if (!db.Patients.Any(p => p.Id == SeedPatientId))
        {
            db.Patients.Add(new Patient
            {
                Id = SeedPatientId,
                Name = "Seed Patient Integration",
                Species = AnimalSpecies.Dog,
                Breed = "Beagle",
                DateOfBirth = new DateTime(2019, 5, 10),
                Weight = 13.2m,
                MicrochipId = "SEED-MC-INTEGRATION",
                Color = "Brown",
                OwnerId = SeedOwnerId
            });
        }

        if (!db.Vets.Any(v => v.Id == SeedVetId))
        {
            db.Vets.Add(new Vet
            {
                Id = SeedVetId,
                FirstName = "Seed",
                LastName = "VetIntegration",
                Specialization = VeterinarySpecialization.GeneralPractice,
                LicenseNumber = "SEED-VET-INTEGRATION",
                YearsOfExperience = 9,
                Phone = "092 700 7000",
                HourlyRate = 88m,
                ClinicId = SeedClinicId
            });
        }

        if (!db.Services.Any(s => s.Id == SeedServiceId))
        {
            db.Services.Add(new Service
            {
                Id = SeedServiceId,
                Name = "Seed Service Integration",
                Description = "Seed service",
                Price = 30m,
                EstimatedDurationMinutes = 20
            });
        }

        db.SaveChanges();
    }
}
