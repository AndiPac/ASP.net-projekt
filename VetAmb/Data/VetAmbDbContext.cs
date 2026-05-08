using Microsoft.EntityFrameworkCore;
using VetAmb.Models;

namespace VetAmb.Data
{
    public class VetAmbDbContext : DbContext
    {
        public VetAmbDbContext(DbContextOptions<VetAmbDbContext> options)
            : base(options)
        {
        }

        public DbSet<Clinic> Clinics => Set<Clinic>();
        public DbSet<Owner> Owners => Set<Owner>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Vet> Vets => Set<Vet>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Weight)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Vet>()
                .Property(v => v.HourlyRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Vet)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppointmentService>(entity =>
            {
                entity.HasOne(x => x.Appointment)
                    .WithMany(x => x.AppointmentServices)
                    .HasForeignKey(x => x.AppointmentId);

                entity.HasOne(x => x.Service)
                    .WithMany(x => x.AppointmentServices)
                    .HasForeignKey(x => x.ServiceId);

                entity.HasIndex(x => new { x.AppointmentId, x.ServiceId }).IsUnique();
            });

            modelBuilder.Entity<Clinic>().HasData(
                new Clinic
                {
                    Id = 1,
                    Name = "Paws & Claws Vet",
                    Address = "123 Main St",
                    Phone = "555-1000",
                    Email = "info@pawsclaws.com",
                    FoundationDate = new DateTime(2010, 3, 15),
                    MaxCapacity = 50,
                    RegistrationNumber = "CLN-001"
                },
                new Clinic
                {
                    Id = 2,
                    Name = "Happy Tails Clinic",
                    Address = "456 Park Rd",
                    Phone = "555-3000",
                    Email = "contact@happytails.com",
                    FoundationDate = new DateTime(2015, 7, 1),
                    MaxCapacity = 30,
                    RegistrationNumber = "CLN-002"
                },
                new Clinic
                {
                    Id = 3,
                    Name = "VetCare Plus",
                    Address = "789 River Blvd",
                    Phone = "555-5000",
                    Email = "hello@vetcareplus.com",
                    FoundationDate = new DateTime(2018, 11, 22),
                    MaxCapacity = 40,
                    RegistrationNumber = "CLN-003"
                }
            );

            modelBuilder.Entity<Vet>().HasData(
                new Vet { Id = 1, FirstName = "Ana", LastName = "Kovač", Specialization = VeterinarySpecialization.GeneralPractice, LicenseNumber = "VET-101", YearsOfExperience = 10, Phone = "555-1001", HourlyRate = 80m, ClinicId = 1 },
                new Vet { Id = 2, FirstName = "Marko", LastName = "Novak", Specialization = VeterinarySpecialization.Surgery, LicenseNumber = "VET-102", YearsOfExperience = 7, Phone = "555-1002", HourlyRate = 100m, ClinicId = 1 },
                new Vet { Id = 3, FirstName = "Luka", LastName = "Jurić", Specialization = VeterinarySpecialization.Cardiology, LicenseNumber = "VET-201", YearsOfExperience = 12, Phone = "555-3001", HourlyRate = 120m, ClinicId = 2 },
                new Vet { Id = 4, FirstName = "Maja", LastName = "Tomić", Specialization = VeterinarySpecialization.Dermatology, LicenseNumber = "VET-202", YearsOfExperience = 5, Phone = "555-3002", HourlyRate = 90m, ClinicId = 2 },
                new Vet { Id = 5, FirstName = "Elena", LastName = "Matić", Specialization = VeterinarySpecialization.Orthopedics, LicenseNumber = "VET-301", YearsOfExperience = 15, Phone = "555-5001", HourlyRate = 130m, ClinicId = 3 },
                new Vet { Id = 6, FirstName = "Dario", LastName = "Šimić", Specialization = VeterinarySpecialization.Dentistry, LicenseNumber = "VET-302", YearsOfExperience = 8, Phone = "555-5002", HourlyRate = 95m, ClinicId = 3 }
            );

            modelBuilder.Entity<Owner>().HasData(
                new Owner { Id = 1, FirstName = "Ivan", LastName = "Horvat", Email = "ivan@mail.com", Phone = "555-2001", Address = "10 Oak Ave", RegistrationDate = new DateTime(2020, 1, 5), IdNumber = "OWN-001", ClinicId = 1 },
                new Owner { Id = 2, FirstName = "Petra", LastName = "Babić", Email = "petra@mail.com", Phone = "555-2002", Address = "22 Elm St", RegistrationDate = new DateTime(2021, 6, 12), IdNumber = "OWN-002", ClinicId = 1 },
                new Owner { Id = 3, FirstName = "Tomislav", LastName = "Knežević", Email = "tomi@mail.com", Phone = "555-4001", Address = "5 Pine Ln", RegistrationDate = new DateTime(2019, 11, 20), IdNumber = "OWN-003", ClinicId = 2 },
                new Owner { Id = 4, FirstName = "Sara", LastName = "Petrović", Email = "sara@mail.com", Phone = "555-4002", Address = "8 Birch Dr", RegistrationDate = new DateTime(2022, 2, 14), IdNumber = "OWN-004", ClinicId = 2 },
                new Owner { Id = 5, FirstName = "Nina", LastName = "Vuković", Email = "nina@mail.com", Phone = "555-6001", Address = "3 Cedar Ct", RegistrationDate = new DateTime(2021, 9, 1), IdNumber = "OWN-005", ClinicId = 3 },
                new Owner { Id = 6, FirstName = "Filip", LastName = "Radić", Email = "filip@mail.com", Phone = "555-6002", Address = "17 Maple Way", RegistrationDate = new DateTime(2023, 4, 30), IdNumber = "OWN-006", ClinicId = 3 }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Name = "Rex", Species = AnimalSpecies.Dog, Breed = "German Shepherd", DateOfBirth = new DateTime(2018, 5, 1), Weight = 32.5m, MicrochipId = "MC-0001", Color = "Black/Tan", OwnerId = 1 },
                new Patient { Id = 2, Name = "Mici", Species = AnimalSpecies.Cat, Breed = "Siamese", DateOfBirth = new DateTime(2020, 8, 20), Weight = 4.2m, MicrochipId = "MC-0002", Color = "Cream", OwnerId = 1 },
                new Patient { Id = 3, Name = "Buddy", Species = AnimalSpecies.Dog, Breed = "Labrador", DateOfBirth = new DateTime(2019, 3, 10), Weight = 28.0m, MicrochipId = "MC-0003", Color = "Golden", OwnerId = 2 },
                new Patient { Id = 4, Name = "Luna", Species = AnimalSpecies.Cat, Breed = "Persian", DateOfBirth = new DateTime(2021, 1, 15), Weight = 5.0m, MicrochipId = "MC-0004", Color = "White", OwnerId = 3 },
                new Patient { Id = 5, Name = "Kiki", Species = AnimalSpecies.Bird, Breed = "Cockatiel", DateOfBirth = new DateTime(2022, 4, 3), Weight = 0.1m, MicrochipId = "MC-0005", Color = "Yellow", OwnerId = 3 },
                new Patient { Id = 6, Name = "Rocky", Species = AnimalSpecies.Rabbit, Breed = "Holland Lop", DateOfBirth = new DateTime(2023, 6, 8), Weight = 1.8m, MicrochipId = "MC-0006", Color = "Brown", OwnerId = 4 },
                new Patient { Id = 7, Name = "Max", Species = AnimalSpecies.Dog, Breed = "Poodle", DateOfBirth = new DateTime(2020, 12, 25), Weight = 7.5m, MicrochipId = "MC-0007", Color = "White", OwnerId = 5 },
                new Patient { Id = 8, Name = "Coco", Species = AnimalSpecies.Hamster, Breed = "Syrian", DateOfBirth = new DateTime(2024, 2, 14), Weight = 0.15m, MicrochipId = "MC-0008", Color = "Orange", OwnerId = 5 },
                new Patient { Id = 9, Name = "Zara", Species = AnimalSpecies.Cat, Breed = "Maine Coon", DateOfBirth = new DateTime(2019, 7, 7), Weight = 6.3m, MicrochipId = "MC-0009", Color = "Tabby", OwnerId = 6 }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "General Checkup", Description = "Routine health examination", Price = 30m, EstimatedDurationMinutes = 30 },
                new Service { Id = 2, Name = "Vaccination", Description = "Standard vaccination", Price = 25m, EstimatedDurationMinutes = 15 },
                new Service { Id = 3, Name = "Minor Surgery", Description = "Minor surgical procedure", Price = 150m, EstimatedDurationMinutes = 60 },
                new Service { Id = 4, Name = "Dental Cleaning", Description = "Professional teeth cleaning", Price = 80m, EstimatedDurationMinutes = 45 },
                new Service { Id = 5, Name = "X-Ray", Description = "Diagnostic imaging", Price = 60m, EstimatedDurationMinutes = 20 }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, AppointmentDateTime = new DateTime(2026, 5, 6, 9, 0, 0), Reason = "Annual checkup", Status = AppointmentStatus.Completed, Notes = "All good", RescheduleReason = null, PatientId = 1, VetId = 1 },
                new Appointment { Id = 2, AppointmentDateTime = new DateTime(2026, 5, 3, 14, 0, 0), Reason = "Lump removal", Status = AppointmentStatus.Completed, Notes = "Successful", RescheduleReason = null, PatientId = 3, VetId = 2 },
                new Appointment { Id = 3, AppointmentDateTime = new DateTime(2026, 5, 7, 10, 0, 0), Reason = "Routine visit", Status = AppointmentStatus.Completed, Notes = "Healthy", RescheduleReason = null, PatientId = 4, VetId = 3 },
                new Appointment { Id = 4, AppointmentDateTime = new DateTime(2026, 4, 18, 11, 0, 0), Reason = "Dental issues", Status = AppointmentStatus.Completed, Notes = "Treated", RescheduleReason = null, PatientId = 7, VetId = 6 },
                new Appointment { Id = 5, AppointmentDateTime = new DateTime(2026, 5, 5, 15, 0, 0), Reason = "Leg pain", Status = AppointmentStatus.Rescheduled, Notes = "Rescheduled to May 12, 2026 - 15:00", RescheduleReason = "Owner unavailable due to travel", PatientId = 9, VetId = 5 },
                new Appointment { Id = 6, AppointmentDateTime = new DateTime(2026, 5, 2, 13, 0, 0), Reason = "Teeth cleaning", Status = AppointmentStatus.Completed, Notes = "Done", RescheduleReason = null, PatientId = 2, VetId = 1 }
            );

            modelBuilder.Entity<AppointmentService>().HasData(
                new AppointmentService { Id = 1, AppointmentId = 1, ServiceId = 1 },
                new AppointmentService { Id = 2, AppointmentId = 1, ServiceId = 2 },
                new AppointmentService { Id = 3, AppointmentId = 2, ServiceId = 3 },
                new AppointmentService { Id = 4, AppointmentId = 3, ServiceId = 1 },
                new AppointmentService { Id = 5, AppointmentId = 4, ServiceId = 4 },
                new AppointmentService { Id = 6, AppointmentId = 4, ServiceId = 5 },
                new AppointmentService { Id = 7, AppointmentId = 5, ServiceId = 5 },
                new AppointmentService { Id = 8, AppointmentId = 6, ServiceId = 4 }
            );

            modelBuilder.Entity<MedicalRecord>().HasData(
                new MedicalRecord { Id = 1, Diagnosis = "Healthy - routine exam", Treatment = "None required", RecordDate = new DateTime(2026, 5, 6), Notes = "Annual visit - all vitals normal", PatientId = 1 },
                new MedicalRecord { Id = 2, Diagnosis = "Lipoma (benign lump)", Treatment = "Surgical removal", RecordDate = new DateTime(2026, 5, 3), Notes = "Post-op recovery good", PatientId = 3 },
                new MedicalRecord { Id = 3, Diagnosis = "Healthy - routine exam", Treatment = "None required", RecordDate = new DateTime(2026, 5, 7), Notes = "Weight stable", PatientId = 4 },
                new MedicalRecord { Id = 4, Diagnosis = "Periodontal disease grade 2", Treatment = "Professional dental cleaning", RecordDate = new DateTime(2026, 4, 18), Notes = "Follow-up in 6 months", PatientId = 7 },
                new MedicalRecord { Id = 5, Diagnosis = "Dental tartar buildup", Treatment = "Teeth cleaning", RecordDate = new DateTime(2026, 5, 2), Notes = "Resolved", PatientId = 2 }
            );
        }
    }
}
