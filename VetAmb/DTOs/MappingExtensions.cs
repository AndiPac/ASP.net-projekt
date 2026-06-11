using System.Collections.Generic;
using System.Linq;
using VetAmb.Models;

#nullable enable

namespace VetAmb.DTOs
{
    public static class MappingExtensions
    {
        public static ClinicDTO? ToDTO(this Clinic clinic)
        {
            if (clinic == null) return null;

            return new ClinicDTO
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = clinic.Address,
                Phone = clinic.Phone,
                Email = clinic.Email,
                FoundationDate = clinic.FoundationDate,
                MaxCapacity = clinic.MaxCapacity,
                RegistrationNumber = clinic.RegistrationNumber
            };
        }

        public static VetDTO? ToDTO(this Vet vet)
        {
            if (vet == null) return null;

            return new VetDTO
            {
                Id = vet.Id,
                FirstName = vet.FirstName,
                LastName = vet.LastName,
                Specialization = vet.Specialization.GetDisplayName(),
                LicenseNumber = vet.LicenseNumber,
                YearsOfExperience = vet.YearsOfExperience,
                Phone = vet.Phone,
                HourlyRate = vet.HourlyRate,
                ClinicId = vet.ClinicId,
                Clinic = vet.Clinic?.ToDTO()
            };
        }

        public static OwnerDTO? ToDTO(this Owner owner)
        {
            if (owner == null) return null;

            return new OwnerDTO
            {
                Id = owner.Id,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Email = owner.Email,
                Phone = owner.Phone,
                Address = owner.Address,
                RegistrationDate = owner.RegistrationDate,
                IdNumber = owner.IdNumber,
                ClinicId = owner.ClinicId,
                Clinic = owner.Clinic?.ToDTO()
            };
        }

        public static PatientDTO? ToDTO(this Patient patient)
        {
            if (patient == null) return null;

            return new PatientDTO
            {
                Id = patient.Id,
                Name = patient.Name,
                Species = patient.Species.GetDisplayName(),
                Breed = patient.Breed,
                DateOfBirth = patient.DateOfBirth,
                Weight = patient.Weight,
                MicrochipId = patient.MicrochipId,
                Color = patient.Color,
                OwnerId = patient.OwnerId,
                Owner = patient.Owner?.ToDTO()
            };
        }

        public static ServiceDTO? ToDTO(this Service service)
        {
            if (service == null) return null;

            return new ServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                EstimatedDurationMinutes = service.EstimatedDurationMinutes
            };
        }

        public static MedicalRecordDTO? ToDTO(this MedicalRecord medicalRecord)
        {
            if (medicalRecord == null) return null;

            return new MedicalRecordDTO
            {
                Id = medicalRecord.Id,
                Diagnosis = medicalRecord.Diagnosis,
                Treatment = medicalRecord.Treatment,
                RecordDate = medicalRecord.RecordDate,
                Notes = medicalRecord.Notes,
                PatientId = medicalRecord.PatientId,
                Patient = medicalRecord.Patient?.ToDTO()
            };
        }

        public static AppointmentDTO? ToDTO(this Appointment appointment)
        {
            if (appointment == null) return null;

            return new AppointmentDTO
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Reason = appointment.Reason,
                Status = appointment.Status.GetDisplayName(),
                Notes = appointment.Notes,
                RescheduleReason = appointment.RescheduleReason,
                PatientId = appointment.PatientId,
                Patient = appointment.Patient?.ToDTO(),
                VetId = appointment.VetId,
                Vet = appointment.Vet?.ToDTO(),
                ServiceIds = appointment.AppointmentServices?
                    .Where(appointmentService => appointmentService != null)
                    .Select(appointmentService => appointmentService.ServiceId)
                    .ToList() ?? new List<int>(),
                Services = appointment.AppointmentServices?
                    .Where(appointmentService => appointmentService?.Service != null)
                    .Select(appointmentService => appointmentService.Service!.ToDTO())
                    .Where(serviceDto => serviceDto != null)
                    .Cast<ServiceDTO>()
                    .ToList() ?? new List<ServiceDTO>()
            };
        }
    }
}
