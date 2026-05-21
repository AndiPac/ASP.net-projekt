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
                .HasQueryFilter(a => a.DeletedAt == null);

            modelBuilder.Entity<Patient>()
                .HasQueryFilter(p => p.DeletedAt == null);

            modelBuilder.Entity<Owner>()
                .HasQueryFilter(o => o.DeletedAt == null);

            modelBuilder.Entity<Vet>()
                .HasQueryFilter(v => v.DeletedAt == null);

            modelBuilder.Entity<Service>()
                .HasQueryFilter(s => s.DeletedAt == null);

            modelBuilder.Entity<Clinic>()
                .HasQueryFilter(c => c.DeletedAt == null);

            modelBuilder.Entity<MedicalRecord>()
                .HasQueryFilter(m => m.DeletedAt == null);

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
                    Address = "Ilica 42, Zagreb",
                    Phone = "01 234 5678",
                    Email = "info@vetklinika.hr",
                    FoundationDate = new DateTime(2010, 3, 15),
                    MaxCapacity = 50,
                    RegistrationNumber = "CLN-001"
                },
                new Clinic
                {
                    Id = 2,
                    Name = "Happy Tails Clinic",
                    Address = "Varšavska 15, Zagreb",
                    Phone = "01 345 6789",
                    Email = "kontakt@vetambulanta.hr",
                    FoundationDate = new DateTime(2015, 7, 1),
                    MaxCapacity = 30,
                    RegistrationNumber = "CLN-002"
                },
                new Clinic
                {
                    Id = 3,
                    Name = "VetCare Plus",
                    Address = "Marmontova 3, Split",
                    Phone = "021 456 7890",
                    Email = "info@vetcentar-split.hr",
                    FoundationDate = new DateTime(2018, 11, 22),
                    MaxCapacity = 40,
                    RegistrationNumber = "CLN-003"
                }
            );

            modelBuilder.Entity<Vet>().HasData(
                new Vet { Id = 1, FirstName = "Ana", LastName = "Kovač", Specialization = VeterinarySpecialization.GeneralPractice, LicenseNumber = "VET-101", YearsOfExperience = 10, Phone = "091 234 1001", HourlyRate = 80m, ClinicId = 1 },
                new Vet { Id = 2, FirstName = "Marko", LastName = "Novak", Specialization = VeterinarySpecialization.Surgery, LicenseNumber = "VET-102", YearsOfExperience = 7, Phone = "092 456 1002", HourlyRate = 100m, ClinicId = 1 },
                new Vet { Id = 3, FirstName = "Luka", LastName = "Jurić", Specialization = VeterinarySpecialization.Cardiology, LicenseNumber = "VET-201", YearsOfExperience = 12, Phone = "095 678 3001", HourlyRate = 120m, ClinicId = 2 },
                new Vet { Id = 4, FirstName = "Maja", LastName = "Tomić", Specialization = VeterinarySpecialization.Dermatology, LicenseNumber = "VET-202", YearsOfExperience = 5, Phone = "099 321 3002", HourlyRate = 90m, ClinicId = 2 },
                new Vet { Id = 5, FirstName = "Elena", LastName = "Matić", Specialization = VeterinarySpecialization.Orthopedics, LicenseNumber = "VET-301", YearsOfExperience = 15, Phone = "098 123 5001", HourlyRate = 130m, ClinicId = 3 },
                new Vet { Id = 6, FirstName = "Dario", LastName = "Šimić", Specialization = VeterinarySpecialization.Dentistry, LicenseNumber = "VET-302", YearsOfExperience = 8, Phone = "091 765 5002", HourlyRate = 95m, ClinicId = 3 }
            );

            modelBuilder.Entity<Owner>().HasData(
                new Owner { Id = 1, FirstName = "Ivan", LastName = "Horvat", Email = "ivan.horvat@gmail.com", Phone = "091 234 5678", Address = "Ilica 10, Zagreb", RegistrationDate = new DateTime(2020, 1, 5), IdNumber = "OWN-001", ClinicId = 1 },
                new Owner { Id = 2, FirstName = "Petra", LastName = "Babić", Email = "petra.babic@gmail.com", Phone = "098 765 4321", Address = "Maksimirska 22, Zagreb", RegistrationDate = new DateTime(2021, 6, 12), IdNumber = "OWN-002", ClinicId = 1 },
                new Owner { Id = 3, FirstName = "Tomislav", LastName = "Knežević", Email = "tomislav.knezevic@gmail.com", Phone = "095 111 2233", Address = "Gajeva 5, Zagreb", RegistrationDate = new DateTime(2019, 11, 20), IdNumber = "OWN-003", ClinicId = 2 },
                new Owner { Id = 4, FirstName = "Sara", LastName = "Petrović", Email = "sara.petrovic@gmail.com", Phone = "099 876 5432", Address = "Šubićeva 8, Zagreb", RegistrationDate = new DateTime(2022, 2, 14), IdNumber = "OWN-004", ClinicId = 2 },
                new Owner { Id = 5, FirstName = "Nina", LastName = "Vuković", Email = "nina.vukovic@gmail.com", Phone = "091 500 6001", Address = "Marmontova 3, Split", RegistrationDate = new DateTime(2021, 9, 1), IdNumber = "OWN-005", ClinicId = 3 },
                new Owner { Id = 6, FirstName = "Filip", LastName = "Radić", Email = "filip.radic@gmail.com", Phone = "098 600 1700", Address = "Obala 17, Split", RegistrationDate = new DateTime(2023, 4, 30), IdNumber = "OWN-006", ClinicId = 3 }
            );

            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Name = "Rex", Species = AnimalSpecies.Dog, Breed = "Njemački ovčar", DateOfBirth = new DateTime(2018, 5, 1), Weight = 32.5m, MicrochipId = "MC-0001", Color = "Crno-smeđa", OwnerId = 1 },
                new Patient { Id = 2, Name = "Mici", Species = AnimalSpecies.Cat, Breed = "Sijamska", DateOfBirth = new DateTime(2020, 8, 20), Weight = 4.2m, MicrochipId = "MC-0002", Color = "Kremasta", OwnerId = 1 },
                new Patient { Id = 3, Name = "Bruno", Species = AnimalSpecies.Dog, Breed = "Labrador retriver", DateOfBirth = new DateTime(2019, 3, 10), Weight = 28.0m, MicrochipId = "MC-0003", Color = "Zlatna", OwnerId = 2 },
                new Patient { Id = 4, Name = "Luna", Species = AnimalSpecies.Cat, Breed = "Perzijska", DateOfBirth = new DateTime(2021, 1, 15), Weight = 5.0m, MicrochipId = "MC-0004", Color = "Bijela", OwnerId = 3 },
                new Patient { Id = 5, Name = "Kiki", Species = AnimalSpecies.Bird, Breed = "Kakadu", DateOfBirth = new DateTime(2022, 4, 3), Weight = 0.1m, MicrochipId = "MC-0005", Color = "Žuta", OwnerId = 3 },
                new Patient { Id = 6, Name = "Šarko", Species = AnimalSpecies.Rabbit, Breed = "Patuljasti kunić", DateOfBirth = new DateTime(2023, 6, 8), Weight = 1.8m, MicrochipId = "MC-0006", Color = "Smeđa", OwnerId = 4 },
                new Patient { Id = 7, Name = "Pahuljica", Species = AnimalSpecies.Dog, Breed = "Pudl", DateOfBirth = new DateTime(2020, 12, 25), Weight = 7.5m, MicrochipId = "MC-0007", Color = "Bijela", OwnerId = 5 },
                new Patient { Id = 8, Name = "Hrčko", Species = AnimalSpecies.Hamster, Breed = "Sirijski hrčak", DateOfBirth = new DateTime(2024, 2, 14), Weight = 0.15m, MicrochipId = "MC-0008", Color = "Narančasta", OwnerId = 5 },
                new Patient { Id = 9, Name = "Zara", Species = AnimalSpecies.Cat, Breed = "Maine Coon", DateOfBirth = new DateTime(2019, 7, 7), Weight = 6.3m, MicrochipId = "MC-0009", Color = "Tigrasta", OwnerId = 6 }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Opći pregled", Description = "Rutinski zdravstveni pregled životinje", Price = 30m, EstimatedDurationMinutes = 30 },
                new Service { Id = 2, Name = "Cijepljenje", Description = "Standardno cijepljenje prema protokolu", Price = 25m, EstimatedDurationMinutes = 15 },
                new Service { Id = 3, Name = "Manji kirurški zahvat", Description = "Manja kirurška intervencija", Price = 150m, EstimatedDurationMinutes = 60 },
                new Service { Id = 4, Name = "Čišćenje zubi", Description = "Profesionalno čišćenje zubnog kamenca", Price = 80m, EstimatedDurationMinutes = 45 },
                new Service { Id = 5, Name = "Rendgensko snimanje", Description = "Dijagnostičko slikanje rentgenom", Price = 60m, EstimatedDurationMinutes = 20 }
            );

            modelBuilder.Entity<Appointment>().HasData(
                new Appointment { Id = 1, AppointmentDateTime = new DateTime(2026, 5, 6, 9, 0, 0), Reason = "Godišnji pregled", Status = AppointmentStatus.Completed, Notes = "Sve u redu, vitalni znakovi uredni", RescheduleReason = null, PatientId = 1, VetId = 1 },
                new Appointment { Id = 2, AppointmentDateTime = new DateTime(2026, 5, 3, 14, 0, 0), Reason = "Uklanjanje lipoma", Status = AppointmentStatus.Completed, Notes = "Zahvat uspješan, oporavak teče uredno", RescheduleReason = null, PatientId = 3, VetId = 2 },
                new Appointment { Id = 3, AppointmentDateTime = new DateTime(2026, 5, 7, 10, 0, 0), Reason = "Rutinski pregled", Status = AppointmentStatus.Completed, Notes = "Zdrava, bez promjena", RescheduleReason = null, PatientId = 4, VetId = 3 },
                new Appointment { Id = 4, AppointmentDateTime = new DateTime(2026, 4, 18, 11, 0, 0), Reason = "Problemi sa zubima", Status = AppointmentStatus.Completed, Notes = "Zubni kamenac uklonjen, preporučuje se kontrola za 6 mj.", RescheduleReason = null, PatientId = 7, VetId = 6 },
                new Appointment { Id = 5, AppointmentDateTime = new DateTime(2026, 5, 5, 15, 0, 0), Reason = "Bol u nozi", Status = AppointmentStatus.Rescheduled, Notes = "Odgođeno na 12. svibnja 2026. – 15:00", RescheduleReason = "Vlasnik nedostupan zbog puta", PatientId = 9, VetId = 5 },
                new Appointment { Id = 6, AppointmentDateTime = new DateTime(2026, 5, 2, 13, 0, 0), Reason = "Čišćenje zubi", Status = AppointmentStatus.Completed, Notes = "Zahvat obavljen bez komplikacija", RescheduleReason = null, PatientId = 2, VetId = 1 }
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
                new MedicalRecord { Id = 1, Diagnosis = "Zdrav pacijent – rutinski pregled", Treatment = "Nije potrebno liječenje", RecordDate = new DateTime(2026, 5, 6), Notes = "Godišnji pregled – svi vitalni znakovi uredni", PatientId = 1 },
                new MedicalRecord { Id = 2, Diagnosis = "Lipom (benigna tvorba)", Treatment = "Kirurško uklanjanje", RecordDate = new DateTime(2026, 5, 3), Notes = "Postoperativni oporavak teče uredno", PatientId = 3 },
                new MedicalRecord { Id = 3, Diagnosis = "Zdrav pacijent – rutinski pregled", Treatment = "Nije potrebno liječenje", RecordDate = new DateTime(2026, 5, 7), Notes = "Tjelesna masa stabilna", PatientId = 4 },
                new MedicalRecord { Id = 4, Diagnosis = "Parodontna bolest stupanj 2", Treatment = "Profesionalno čišćenje zubi", RecordDate = new DateTime(2026, 4, 18), Notes = "Kontrola za 6 mjeseci", PatientId = 7 },
                new MedicalRecord { Id = 5, Diagnosis = "Nakupljanje zubnog kamenca", Treatment = "Čišćenje zubi", RecordDate = new DateTime(2026, 5, 2), Notes = "Problem riješen", PatientId = 2 }
            );
        }
    }
}
