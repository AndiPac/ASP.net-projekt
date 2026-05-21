using System;
using System.Collections.Generic;

namespace VetAmb.Models
{
    public static class SeedData
    {
        public static List<Clinic> Clinics { get; }
        public static List<Patient> Patients { get; }
        public static List<Vet> Vets { get; }
        public static List<Owner> Owners { get; }
        public static List<Appointment> Appointments { get; }
        public static List<Service> Services { get; }
        public static List<MedicalRecord> MedicalRecords { get; }

        static SeedData()
        {
            // --- Clinic 1 ---
            var clinic1 = new Clinic
            {
                Id = 1, Name = "Paws & Claws Vet", Address = "Ilica 42, Zagreb",
                Phone = "01 234 5678", Email = "info@vetklinika.hr",
                FoundationDate = new DateTime(2010, 3, 15), MaxCapacity = 50,
                RegistrationNumber = "CLN-001"
            };

            var vet1 = new Vet { Id = 1, FirstName = "Ana", LastName = "Kovač", Specialization = VeterinarySpecialization.GeneralPractice, LicenseNumber = "VET-101", YearsOfExperience = 10, Phone = "091 234 1001", HourlyRate = 80m, ClinicId = 1, Clinic = clinic1 };
            var vet2 = new Vet { Id = 2, FirstName = "Marko", LastName = "Novak", Specialization = VeterinarySpecialization.Surgery, LicenseNumber = "VET-102", YearsOfExperience = 7, Phone = "092 456 1002", HourlyRate = 100m, ClinicId = 1, Clinic = clinic1 };

            var owner1 = new Owner { Id = 1, FirstName = "Ivan", LastName = "Horvat", Email = "ivan.horvat@gmail.com", Phone = "091 234 5678", Address = "Ilica 10, Zagreb", RegistrationDate = new DateTime(2020, 1, 5), IdNumber = "OWN-001", ClinicId = 1, Clinic = clinic1 };
            var owner2 = new Owner { Id = 2, FirstName = "Petra", LastName = "Babić", Email = "petra.babic@gmail.com", Phone = "098 765 4321", Address = "Maksimirska 22, Zagreb", RegistrationDate = new DateTime(2021, 6, 12), IdNumber = "OWN-002", ClinicId = 1, Clinic = clinic1 };

            var patient1 = new Patient { Id = 1, Name = "Rex", Species = AnimalSpecies.Dog, Breed = "Njemački ovčar", DateOfBirth = new DateTime(2018, 5, 1), Weight = 32.5m, MicrochipId = "MC-0001", Color = "Crno-smeđa", OwnerId = 1, Owner = owner1 };
            var patient2 = new Patient { Id = 2, Name = "Mici", Species = AnimalSpecies.Cat, Breed = "Sijamska", DateOfBirth = new DateTime(2020, 8, 20), Weight = 4.2m, MicrochipId = "MC-0002", Color = "Kremasta", OwnerId = 1, Owner = owner1 };
            var patient3 = new Patient { Id = 3, Name = "Bruno", Species = AnimalSpecies.Dog, Breed = "Labrador retriver", DateOfBirth = new DateTime(2019, 3, 10), Weight = 28.0m, MicrochipId = "MC-0003", Color = "Zlatna", OwnerId = 2, Owner = owner2 };

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
                Id = 2, Name = "Happy Tails Clinic", Address = "Varsavska 15, Zagreb",
                Phone = "01 345 6789", Email = "kontakt@vetambulanta.hr",
                FoundationDate = new DateTime(2015, 7, 1), MaxCapacity = 30,
                RegistrationNumber = "CLN-002"
            };

            var vet3 = new Vet { Id = 3, FirstName = "Luka", LastName = "Jurić", Specialization = VeterinarySpecialization.Cardiology, LicenseNumber = "VET-201", YearsOfExperience = 12, Phone = "095 678 3001", HourlyRate = 120m, ClinicId = 2, Clinic = clinic2 };
            var vet4 = new Vet { Id = 4, FirstName = "Maja", LastName = "Tomić", Specialization = VeterinarySpecialization.Dermatology, LicenseNumber = "VET-202", YearsOfExperience = 5, Phone = "099 321 3002", HourlyRate = 90m, ClinicId = 2, Clinic = clinic2 };

            var owner3 = new Owner { Id = 3, FirstName = "Tomislav", LastName = "Knežević", Email = "tomislav.knezevic@gmail.com", Phone = "095 111 2233", Address = "Gajeva 5, Zagreb", RegistrationDate = new DateTime(2019, 11, 20), IdNumber = "OWN-003", ClinicId = 2, Clinic = clinic2 };
            var owner4 = new Owner { Id = 4, FirstName = "Sara", LastName = "Petrović", Email = "sara.petrovic@gmail.com", Phone = "099 876 5432", Address = "Šubićeva 8, Zagreb", RegistrationDate = new DateTime(2022, 2, 14), IdNumber = "OWN-004", ClinicId = 2, Clinic = clinic2 };

            var patient4 = new Patient { Id = 4, Name = "Luna", Species = AnimalSpecies.Cat, Breed = "Perzijska", DateOfBirth = new DateTime(2021, 1, 15), Weight = 5.0m, MicrochipId = "MC-0004", Color = "Bijela", OwnerId = 3, Owner = owner3 };
            var patient5 = new Patient { Id = 5, Name = "Kiki", Species = AnimalSpecies.Bird, Breed = "Kakadu", DateOfBirth = new DateTime(2022, 4, 3), Weight = 0.1m, MicrochipId = "MC-0005", Color = "Žuta", OwnerId = 3, Owner = owner3 };
            var patient6 = new Patient { Id = 6, Name = "Šarko", Species = AnimalSpecies.Rabbit, Breed = "Patuljasti kunić", DateOfBirth = new DateTime(2023, 6, 8), Weight = 1.8m, MicrochipId = "MC-0006", Color = "Smeđa", OwnerId = 4, Owner = owner4 };

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
                Id = 3, Name = "VetCare Plus", Address = "Marmontova 3, Split",
                Phone = "021 456 7890", Email = "info@vetcentar-split.hr",
                FoundationDate = new DateTime(2018, 11, 22), MaxCapacity = 40,
                RegistrationNumber = "CLN-003"
            };

            var vet5 = new Vet { Id = 5, FirstName = "Elena", LastName = "Matić", Specialization = VeterinarySpecialization.Orthopedics, LicenseNumber = "VET-301", YearsOfExperience = 15, Phone = "098 123 5001", HourlyRate = 130m, ClinicId = 3, Clinic = clinic3 };
            var vet6 = new Vet { Id = 6, FirstName = "Dario", LastName = "Šimić", Specialization = VeterinarySpecialization.Dentistry, LicenseNumber = "VET-302", YearsOfExperience = 8, Phone = "091 765 5002", HourlyRate = 95m, ClinicId = 3, Clinic = clinic3 };

            var owner5 = new Owner { Id = 5, FirstName = "Nina", LastName = "Vuković", Email = "nina.vukovic@gmail.com", Phone = "091 500 6001", Address = "Marmontova 3, Split", RegistrationDate = new DateTime(2021, 9, 1), IdNumber = "OWN-005", ClinicId = 3, Clinic = clinic3 };
            var owner6 = new Owner { Id = 6, FirstName = "Filip", LastName = "Radić", Email = "filip.radic@gmail.com", Phone = "098 600 1700", Address = "Obala 17, Split", RegistrationDate = new DateTime(2023, 4, 30), IdNumber = "OWN-006", ClinicId = 3, Clinic = clinic3 };

            var patient7 = new Patient { Id = 7, Name = "Pahuljica", Species = AnimalSpecies.Dog, Breed = "Pudl", DateOfBirth = new DateTime(2020, 12, 25), Weight = 7.5m, MicrochipId = "MC-0007", Color = "Bijela", OwnerId = 5, Owner = owner5 };
            var patient8 = new Patient { Id = 8, Name = "Hrčko", Species = AnimalSpecies.Hamster, Breed = "Sirijski hrček", DateOfBirth = new DateTime(2024, 2, 14), Weight = 0.15m, MicrochipId = "MC-0008", Color = "Narančasta", OwnerId = 5, Owner = owner5 };
            var patient9 = new Patient { Id = 9, Name = "Zara", Species = AnimalSpecies.Cat, Breed = "Maine Coon", DateOfBirth = new DateTime(2019, 7, 7), Weight = 6.3m, MicrochipId = "MC-0009", Color = "Tigrasta", OwnerId = 6, Owner = owner6 };

            owner5.Patients.Add(patient7);
            owner5.Patients.Add(patient8);
            owner6.Patients.Add(patient9);
            clinic3.Vets.Add(vet5);
            clinic3.Vets.Add(vet6);
            clinic3.Owners.Add(owner5);
            clinic3.Owners.Add(owner6);

            // --- Services ---
            var svcCheckup = new Service { Id = 1, Name = "Opći pregled", Description = "Rutinski zdravstveni pregled životinje", Price = 30m, EstimatedDurationMinutes = 30 };
            var svcVaccination = new Service { Id = 2, Name = "Cijepljenje", Description = "Standardno cijepljenje prema protokolu", Price = 25m, EstimatedDurationMinutes = 15 };
            var svcSurgery = new Service { Id = 3, Name = "Manji kirurški zahvat", Description = "Manja kirurška intervencija", Price = 150m, EstimatedDurationMinutes = 60 };
            var svcDental = new Service { Id = 4, Name = "Čišćenje zubi", Description = "Profesionalno čišćenje zubnog kamenca", Price = 80m, EstimatedDurationMinutes = 45 };
            var svcXray = new Service { Id = 5, Name = "Rendgensko snimanje", Description = "Dijagnostičko slikanje rentgenom", Price = 60m, EstimatedDurationMinutes = 20 };

            // --- Appointments ---
            DateTime today = DateTime.Now.Date;

            var appt1 = new Appointment { Id = 1, AppointmentDateTime = today.AddDays(-2).AddHours(9), Reason = "Godišnji pregled", Status = AppointmentStatus.Completed, Notes = "Sve u redu, vitalni znakovi uredni", PatientId = 1, Patient = patient1, VetId = 1, Vet = vet1 };
            var as1a = new AppointmentService { Id = 1, AppointmentId = 1, Appointment = appt1, ServiceId = 1, Service = svcCheckup };
            var as1b = new AppointmentService { Id = 2, AppointmentId = 1, Appointment = appt1, ServiceId = 2, Service = svcVaccination };
            appt1.AppointmentServices.Add(as1a);
            appt1.AppointmentServices.Add(as1b);
            svcCheckup.AppointmentServices.Add(as1a);
            svcVaccination.AppointmentServices.Add(as1b);

            var appt2 = new Appointment { Id = 2, AppointmentDateTime = today.AddDays(-5).AddHours(14), Reason = "Uklanjanje lipoma", Status = AppointmentStatus.Completed, Notes = "Zahvat uspješan, oporavak teče uredno", PatientId = 3, Patient = patient3, VetId = 2, Vet = vet2 };
            var as2a = new AppointmentService { Id = 3, AppointmentId = 2, Appointment = appt2, ServiceId = 3, Service = svcSurgery };
            appt2.AppointmentServices.Add(as2a);
            svcSurgery.AppointmentServices.Add(as2a);

            var appt3 = new Appointment { Id = 3, AppointmentDateTime = today.AddDays(-1).AddHours(10), Reason = "Rutinski pregled", Status = AppointmentStatus.Completed, Notes = "Zdrava, bez promjena", PatientId = 4, Patient = patient4, VetId = 3, Vet = vet3 };
            var as3a = new AppointmentService { Id = 4, AppointmentId = 3, Appointment = appt3, ServiceId = 1, Service = svcCheckup };
            appt3.AppointmentServices.Add(as3a);
            svcCheckup.AppointmentServices.Add(as3a);

            var appt4 = new Appointment { Id = 4, AppointmentDateTime = today.AddDays(-20).AddHours(11), Reason = "Problemi sa zubima", Status = AppointmentStatus.Completed, Notes = "Zubni kamenac uklonjen, preporuča se kontrola za 6 mj.", PatientId = 7, Patient = patient7, VetId = 6, Vet = vet6 };
            var as4a = new AppointmentService { Id = 5, AppointmentId = 4, Appointment = appt4, ServiceId = 4, Service = svcDental };
            var as4b = new AppointmentService { Id = 6, AppointmentId = 4, Appointment = appt4, ServiceId = 5, Service = svcXray };
            appt4.AppointmentServices.Add(as4a);
            appt4.AppointmentServices.Add(as4b);
            svcDental.AppointmentServices.Add(as4a);
            svcXray.AppointmentServices.Add(as4b);

            var appt5 = new Appointment { Id = 5, AppointmentDateTime = today.AddDays(-3).AddHours(15), Reason = "Bol u nozi", Status = AppointmentStatus.Rescheduled, RescheduleReason = "Vlasnik nedostupan zbog puta", Notes = "Odgođeno na " + today.AddDays(4).AddHours(15).ToString("dd. MMMM yyyy. – HH:mm"), PatientId = 9, Patient = patient9, VetId = 5, Vet = vet5 };
            var as5a = new AppointmentService { Id = 7, AppointmentId = 5, Appointment = appt5, ServiceId = 5, Service = svcXray };
            appt5.AppointmentServices.Add(as5a);
            svcXray.AppointmentServices.Add(as5a);

            var appt6 = new Appointment { Id = 6, AppointmentDateTime = today.AddDays(-6).AddHours(13), Reason = "Čišćenje zubi", Status = AppointmentStatus.Completed, Notes = "Zahvat obavljen bez komplikacija", PatientId = 2, Patient = patient2, VetId = 1, Vet = vet1 };
            var as6a = new AppointmentService { Id = 8, AppointmentId = 6, Appointment = appt6, ServiceId = 4, Service = svcDental };
            appt6.AppointmentServices.Add(as6a);
            svcDental.AppointmentServices.Add(as6a);

            patient1.Appointments.Add(appt1);
            patient3.Appointments.Add(appt2);
            patient4.Appointments.Add(appt3);
            patient7.Appointments.Add(appt4);
            patient9.Appointments.Add(appt5);
            patient2.Appointments.Add(appt6);

            Clinics = new List<Clinic> { clinic1, clinic2, clinic3 };
            Patients = new List<Patient> { patient1, patient2, patient3, patient4, patient5, patient6, patient7, patient8, patient9 };
            Vets = new List<Vet> { vet1, vet2, vet3, vet4, vet5, vet6 };
            Owners = new List<Owner> { owner1, owner2, owner3, owner4, owner5, owner6 };
            Appointments = new List<Appointment> { appt1, appt2, appt3, appt4, appt5, appt6 };
            Services = new List<Service> { svcCheckup, svcVaccination, svcSurgery, svcDental, svcXray };

            // --- Medical Records ---
            var mr1 = new MedicalRecord { Id = 1, Diagnosis = "Zdrav pacijent – rutinski pregled", Treatment = "Nije potrebno liječenje", RecordDate = today.AddDays(-2), Notes = "Godišnji pregled – svi vitalni znakovi uredni", PatientId = 1, Patient = patient1 };
            var mr2 = new MedicalRecord { Id = 2, Diagnosis = "Lipom (benigna tvorba)", Treatment = "Kirurško uklanjanje", RecordDate = today.AddDays(-5), Notes = "Postoperativni oporavak teče uredno", PatientId = 3, Patient = patient3 };
            var mr3 = new MedicalRecord { Id = 3, Diagnosis = "Zdrav pacijent – rutinski pregled", Treatment = "Nije potrebno liječenje", RecordDate = today.AddDays(-1), Notes = "Tjelesna masa stabilna", PatientId = 4, Patient = patient4 };
            var mr4 = new MedicalRecord { Id = 4, Diagnosis = "Parodontna bolest stupanj 2", Treatment = "Profesionalno čišćenje zubi", RecordDate = today.AddDays(-20), Notes = "Kontrola za 6 mjeseci", PatientId = 7, Patient = patient7 };
            var mr5 = new MedicalRecord { Id = 5, Diagnosis = "Nakupljanje zubnog kamenca", Treatment = "Čišćenje zubi", RecordDate = today.AddDays(-6), Notes = "Problem riješen", PatientId = 2, Patient = patient2 };

            patient1.MedicalRecords.Add(mr1);
            patient3.MedicalRecords.Add(mr2);
            patient4.MedicalRecords.Add(mr3);
            patient7.MedicalRecords.Add(mr4);
            patient2.MedicalRecords.Add(mr5);

            MedicalRecords = new List<MedicalRecord> { mr1, mr2, mr3, mr4, mr5 };
        }
    }
}
