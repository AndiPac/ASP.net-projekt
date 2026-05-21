using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Vet class - represents a veterinarian (Complex class with 8 attributes)
    /// </summary>
    public class Vet
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public VeterinarySpecialization Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Phone { get; set; }
        public decimal HourlyRate { get; set; }

        // Foreign key relationship (1-N with Clinic)
        [ForeignKey(nameof(Clinic))]
        public int ClinicId { get; set; }
        public virtual Clinic? Clinic { get; set; }

        // Soft delete — never physically remove vet records.
        public DateTime? DeletedAt { get; set; }

        // Relationships
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
