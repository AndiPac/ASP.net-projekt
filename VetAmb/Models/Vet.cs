using System;
using System.Collections.Generic;

#nullable enable

namespace AspNetProjekt.Models
{
    /// <summary>
    /// Vet class - represents a veterinarian (Complex class with 8 attributes)
    /// </summary>
    public class Vet
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public VeterinarySpecialization Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Phone { get; set; }
        public decimal HourlyRate { get; set; }

        // Foreign key relationship (1-N with Clinic)
        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        // Relationships
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
