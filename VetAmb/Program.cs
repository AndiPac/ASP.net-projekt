using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VetAmb.Models;

namespace VetAmb
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // --- Clinic 1 ---
            var clinic1 = new Clinic
            {
                Id = 1,
                Name = "Paws & Claws Vet",
                Address = "123 Main St",
                Phone = "555-1000",
                Email = "info@pawsclaws.com",
                FoundationDate = new DateTime(2010, 3, 15),
                MaxCapacity = 50,
                RegistrationNumber = "CLN-001"
            };

            var vet1 = new Vet { Id = 1, FirstName = "Ana", LastName = "Kovač", Specialization = VeterinarySpecialization.GeneralPractice, LicenseNumber = "VET-101", YearsOfExperience = 10, Phone = "555-1001", HourlyRate = 80m, ClinicId = 1, Clinic = clinic1 };
            var vet2 = new Vet { Id = 2, FirstName = "Marko", LastName = "Novak", Specialization = VeterinarySpecialization.Surgery, LicenseNumber = "VET-102", YearsOfExperience = 7, Phone = "555-1002", HourlyRate = 100m, ClinicId = 1, Clinic = clinic1 };

            var owner1 = new Owner { Id = 1, FirstName = "Ivan", LastName = "Horvat", Email = "ivan@mail.com", Phone = "555-2001", Address = "10 Oak Ave", RegistrationDate = new DateTime(2020, 1, 5), IdNumber = "OWN-001", ClinicId = 1, Clinic = clinic1 };
            var owner2 = new Owner { Id = 2, FirstName = "Petra", LastName = "Babić", Email = "petra@mail.com", Phone = "555-2002", Address = "22 Elm St", RegistrationDate = new DateTime(2021, 6, 12), IdNumber = "OWN-002", ClinicId = 1, Clinic = clinic1 };

            var patient1 = new Patient { Id = 1, Name = "Rex", Species = AnimalSpecies.Dog, Breed = "German Shepherd", DateOfBirth = new DateTime(2018, 5, 1), Weight = 32.5m, MicrochipId = "MC-0001", Color = "Black/Tan", OwnerId = 1, Owner = owner1 };
            var patient2 = new Patient { Id = 2, Name = "Mici", Species = AnimalSpecies.Cat, Breed = "Siamese", DateOfBirth = new DateTime(2020, 8, 20), Weight = 4.2m, MicrochipId = "MC-0002", Color = "Cream", OwnerId = 1, Owner = owner1 };
            var patient3 = new Patient { Id = 3, Name = "Buddy", Species = AnimalSpecies.Dog, Breed = "Labrador", DateOfBirth = new DateTime(2019, 3, 10), Weight = 28.0m, MicrochipId = "MC-0003", Color = "Golden", OwnerId = 2, Owner = owner2 };

            owner1.Patients.Add(patient1);
            owner1.Patients.Add(patient2);
            owner2.Patients.Add(patient3);
            clinic1.Vets.Add(vet1);
            clinic1.Vets.Add(vet2);
            clinic1.Owners.Add(owner1);
            clinic1.Owners.Add(owner2);

            // --- Clinic 2 ---
            var clinic2 = new Clinic
            {
                Id = 2,
                Name = "Happy Tails Clinic",
                Address = "456 Park Rd",
                Phone = "555-3000",
                Email = "contact@happytails.com",
                FoundationDate = new DateTime(2015, 7, 1),
                MaxCapacity = 30,
                RegistrationNumber = "CLN-002"
            };

            var vet3 = new Vet { Id = 3, FirstName = "Luka", LastName = "Jurić", Specialization = VeterinarySpecialization.Cardiology, LicenseNumber = "VET-201", YearsOfExperience = 12, Phone = "555-3001", HourlyRate = 120m, ClinicId = 2, Clinic = clinic2 };
            var vet4 = new Vet { Id = 4, FirstName = "Maja", LastName = "Tomić", Specialization = VeterinarySpecialization.Dermatology, LicenseNumber = "VET-202", YearsOfExperience = 5, Phone = "555-3002", HourlyRate = 90m, ClinicId = 2, Clinic = clinic2 };

            var owner3 = new Owner { Id = 3, FirstName = "Tomislav", LastName = "Knežević", Email = "tomi@mail.com", Phone = "555-4001", Address = "5 Pine Ln", RegistrationDate = new DateTime(2019, 11, 20), IdNumber = "OWN-003", ClinicId = 2, Clinic = clinic2 };
            var owner4 = new Owner { Id = 4, FirstName = "Sara", LastName = "Petrović", Email = "sara@mail.com", Phone = "555-4002", Address = "8 Birch Dr", RegistrationDate = new DateTime(2022, 2, 14), IdNumber = "OWN-004", ClinicId = 2, Clinic = clinic2 };

            var patient4 = new Patient { Id = 4, Name = "Luna", Species = AnimalSpecies.Cat, Breed = "Persian", DateOfBirth = new DateTime(2021, 1, 15), Weight = 5.0m, MicrochipId = "MC-0004", Color = "White", OwnerId = 3, Owner = owner3 };
            var patient5 = new Patient { Id = 5, Name = "Kiki", Species = AnimalSpecies.Bird, Breed = "Cockatiel", DateOfBirth = new DateTime(2022, 4, 3), Weight = 0.1m, MicrochipId = "MC-0005", Color = "Yellow", OwnerId = 3, Owner = owner3 };
            var patient6 = new Patient { Id = 6, Name = "Rocky", Species = AnimalSpecies.Rabbit, Breed = "Holland Lop", DateOfBirth = new DateTime(2023, 6, 8), Weight = 1.8m, MicrochipId = "MC-0006", Color = "Brown", OwnerId = 4, Owner = owner4 };

            owner3.Patients.Add(patient4);
            owner3.Patients.Add(patient5);
            owner4.Patients.Add(patient6);
            clinic2.Vets.Add(vet3);
            clinic2.Vets.Add(vet4);
            clinic2.Owners.Add(owner3);
            clinic2.Owners.Add(owner4);

            // --- Clinic 3 ---
            var clinic3 = new Clinic
            {
                Id = 3,
                Name = "VetCare Plus",
                Address = "789 River Blvd",
                Phone = "555-5000",
                Email = "hello@vetcareplus.com",
                FoundationDate = new DateTime(2018, 11, 22),
                MaxCapacity = 40,
                RegistrationNumber = "CLN-003"
            };

            var vet5 = new Vet { Id = 5, FirstName = "Elena", LastName = "Matić", Specialization = VeterinarySpecialization.Orthopedics, LicenseNumber = "VET-301", YearsOfExperience = 15, Phone = "555-5001", HourlyRate = 130m, ClinicId = 3, Clinic = clinic3 };
            var vet6 = new Vet { Id = 6, FirstName = "Dario", LastName = "Šimić", Specialization = VeterinarySpecialization.Dentistry, LicenseNumber = "VET-302", YearsOfExperience = 8, Phone = "555-5002", HourlyRate = 95m, ClinicId = 3, Clinic = clinic3 };

            var owner5 = new Owner { Id = 5, FirstName = "Nina", LastName = "Vuković", Email = "nina@mail.com", Phone = "555-6001", Address = "3 Cedar Ct", RegistrationDate = new DateTime(2021, 9, 1), IdNumber = "OWN-005", ClinicId = 3, Clinic = clinic3 };
            var owner6 = new Owner { Id = 6, FirstName = "Filip", LastName = "Radić", Email = "filip@mail.com", Phone = "555-6002", Address = "17 Maple Way", RegistrationDate = new DateTime(2023, 4, 30), IdNumber = "OWN-006", ClinicId = 3, Clinic = clinic3 };

            var patient7 = new Patient { Id = 7, Name = "Max", Species = AnimalSpecies.Dog, Breed = "Poodle", DateOfBirth = new DateTime(2020, 12, 25), Weight = 7.5m, MicrochipId = "MC-0007", Color = "White", OwnerId = 5, Owner = owner5 };
            var patient8 = new Patient { Id = 8, Name = "Coco", Species = AnimalSpecies.Hamster, Breed = "Syrian", DateOfBirth = new DateTime(2024, 2, 14), Weight = 0.15m, MicrochipId = "MC-0008", Color = "Orange", OwnerId = 5, Owner = owner5 };
            var patient9 = new Patient { Id = 9, Name = "Zara", Species = AnimalSpecies.Cat, Breed = "Maine Coon", DateOfBirth = new DateTime(2019, 7, 7), Weight = 6.3m, MicrochipId = "MC-0009", Color = "Tabby", OwnerId = 6, Owner = owner6 };

            owner5.Patients.Add(patient7);
            owner5.Patients.Add(patient8);
            owner6.Patients.Add(patient9);
            clinic3.Vets.Add(vet5);
            clinic3.Vets.Add(vet6);
            clinic3.Owners.Add(owner5);
            clinic3.Owners.Add(owner6);

            // Store all clinics in a List<Clinic>
            List<Clinic> clinics = new List<Clinic> { clinic1, clinic2, clinic3 };

            // Print summary
            foreach (var clinic in clinics)
            {
                Console.WriteLine($"=== {clinic.Name} ({clinic.Address}) ===");
                Console.WriteLine($"  Vets ({clinic.Vets.Count}):");
                foreach (var vet in clinic.Vets)
                {
                    Console.WriteLine($"    - Dr. {vet.FirstName} {vet.LastName} [{vet.Specialization}]");
                }

                Console.WriteLine($"  Owners ({clinic.Owners.Count}):");
                foreach (var owner in clinic.Owners)
                {
                    Console.WriteLine($"    - {owner.FirstName} {owner.LastName}");
                    foreach (var patient in owner.Patients)
                    {
                        Console.WriteLine($"        Pet: {patient.Name} ({patient.Species}, {patient.Breed})");
                    }
                }
                Console.WriteLine();
            }

            // --- Services ---
            var svcCheckup = new Service { Id = 1, Name = "General Checkup", Description = "Routine health examination", Price = 30m, EstimatedDurationMinutes = 30 };
            var svcVaccination = new Service { Id = 2, Name = "Vaccination", Description = "Standard vaccination", Price = 25m, EstimatedDurationMinutes = 15 };
            var svcSurgery = new Service { Id = 3, Name = "Minor Surgery", Description = "Minor surgical procedure", Price = 150m, EstimatedDurationMinutes = 60 };
            var svcDental = new Service { Id = 4, Name = "Dental Cleaning", Description = "Professional teeth cleaning", Price = 80m, EstimatedDurationMinutes = 45 };
            var svcXray = new Service { Id = 5, Name = "X-Ray", Description = "Diagnostic imaging", Price = 60m, EstimatedDurationMinutes = 20 };

            // --- Appointments (mix of dates, statuses, and total prices) ---
            // Today is used as reference for "last 7 days"
            DateTime today = DateTime.Now.Date;

            // Appt 1: Completed 2 days ago, Checkup + Vaccination = 55€ (> 50, within 7 days) --> MATCH
            var appt1 = new Appointment { Id = 1, AppointmentDateTime = today.AddDays(-2).AddHours(9), Reason = "Annual checkup", Status = AppointmentStatus.Completed, Notes = "All good", PatientId = 1, Patient = patient1, VetId = 1, Vet = vet1 };
            var as1a = new AppointmentService { Id = 1, AppointmentId = 1, Appointment = appt1, ServiceId = 1, Service = svcCheckup };
            var as1b = new AppointmentService { Id = 2, AppointmentId = 1, Appointment = appt1, ServiceId = 2, Service = svcVaccination };
            appt1.AppointmentServices.Add(as1a);
            appt1.AppointmentServices.Add(as1b);

            // Appt 2: Completed 5 days ago, Surgery = 150€ (> 50, within 7 days) --> MATCH
            var appt2 = new Appointment { Id = 2, AppointmentDateTime = today.AddDays(-5).AddHours(14), Reason = "Lump removal", Status = AppointmentStatus.Completed, Notes = "Successful", PatientId = 3, Patient = patient3, VetId = 2, Vet = vet2 };
            var as2a = new AppointmentService { Id = 3, AppointmentId = 2, Appointment = appt2, ServiceId = 3, Service = svcSurgery };
            appt2.AppointmentServices.Add(as2a);

            // Appt 3: Completed 1 day ago, Checkup only = 30€ (< 50, within 7 days) --> NO (price too low)
            var appt3 = new Appointment { Id = 3, AppointmentDateTime = today.AddDays(-1).AddHours(10), Reason = "Routine visit", Status = AppointmentStatus.Completed, Notes = "Healthy", PatientId = 4, Patient = patient4, VetId = 3, Vet = vet3 };
            var as3a = new AppointmentService { Id = 4, AppointmentId = 3, Appointment = appt3, ServiceId = 1, Service = svcCheckup };
            appt3.AppointmentServices.Add(as3a);

            // Appt 4: Completed 20 days ago, Dental + X-Ray = 140€ (> 50, but outside 7 days) --> NO (too old)
            var appt4 = new Appointment { Id = 4, AppointmentDateTime = today.AddDays(-20).AddHours(11), Reason = "Dental issues", Status = AppointmentStatus.Completed, Notes = "Treated", PatientId = 7, Patient = patient7, VetId = 6, Vet = vet6 };
            var as4a = new AppointmentService { Id = 5, AppointmentId = 4, Appointment = appt4, ServiceId = 4, Service = svcDental };
            var as4b = new AppointmentService { Id = 6, AppointmentId = 4, Appointment = appt4, ServiceId = 5, Service = svcXray };
            appt4.AppointmentServices.Add(as4a);
            appt4.AppointmentServices.Add(as4b);

            // Appt 5: Scheduled (not completed) 3 days ago, X-Ray = 60€ --> NO (not completed)
            var appt5 = new Appointment { Id = 5, AppointmentDateTime = today.AddDays(-3).AddHours(15), Reason = "Leg pain", Status = AppointmentStatus.Scheduled, Notes = "", PatientId = 9, Patient = patient9, VetId = 5, Vet = vet5 };
            var as5a = new AppointmentService { Id = 7, AppointmentId = 5, Appointment = appt5, ServiceId = 5, Service = svcXray };
            appt5.AppointmentServices.Add(as5a);

            // Appt 6: Completed 6 days ago, Dental = 80€ (> 50, within 7 days) --> MATCH
            var appt6 = new Appointment { Id = 6, AppointmentDateTime = today.AddDays(-6).AddHours(13), Reason = "Teeth cleaning", Status = AppointmentStatus.Completed, Notes = "Done", PatientId = 2, Patient = patient2, VetId = 1, Vet = vet1 };
            var as6a = new AppointmentService { Id = 8, AppointmentId = 6, Appointment = appt6, ServiceId = 4, Service = svcDental };
            appt6.AppointmentServices.Add(as6a);

            List<Appointment> allAppointments = new List<Appointment> { appt1, appt2, appt3, appt4, appt5, appt6 };

            // LINQ: Completed in last 7 days with total price > 50€, sorted by date descending
            var recentExpensiveCompleted = allAppointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .Where(a => a.AppointmentDateTime >= today.AddDays(-7))
                .Where(a => a.AppointmentServices.Sum(s => s.Service!.Price) > 50m)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToList();

            Console.WriteLine("=== Completed appointments (last 7 days, total > 50€) ===");
            foreach (var appt in recentExpensiveCompleted)
            {
                decimal total = appt.AppointmentServices.Sum(s => s.Service!.Price);
                Console.WriteLine($"  [{appt.AppointmentDateTime:yyyy-MM-dd HH:mm}] {appt.Patient!.Name} - {appt.Reason} | Total: {total}€ | Status: {appt.Status}");
            }

            // LINQ: Group appointments by VetId, calculate count and total revenue, order by revenue descending
            var revenueByVet = allAppointments
                .GroupBy(a => a.VetId)
                .Select(g => new
                {
                    VetId = g.Key,
                    VetName = g.First().Vet!.FirstName + " " + g.First().Vet!.LastName,
                    TotalAppointments = g.Count(),
                    TotalRevenue = g.Sum(a => a.AppointmentServices.Sum(s => s.Service!.Price))
                })
                .OrderByDescending(v => v.TotalRevenue)
                .ToList();

            Console.WriteLine("\n=== Revenue by Veterinarian (all appointments) ===");
            foreach (var v in revenueByVet)
            {
                Console.WriteLine($"  Dr. {v.VetName} | Appointments: {v.TotalAppointments} | Revenue: {v.TotalRevenue}€");
            }

            // Test SaveAppointmentAsync
            Console.WriteLine();
            await SaveAppointmentAsync(appt1);
        }

        static async Task SaveAppointmentAsync(Appointment appointment)
        {
            Console.WriteLine($"Saving appointment (Id: {appointment.Id}, Patient: {appointment.Patient?.Name}, Reason: {appointment.Reason})...");
            await Task.Delay(2000);
            Console.WriteLine($"Appointment (Id: {appointment.Id}) saved successfully.");
        }
    }
}
