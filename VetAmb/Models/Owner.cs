using System;
using System.Collections.Generic;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Owner class - represents a pet owner (Complex class with 8 attributes)
    /// </summary>
    public class Owner
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? IdNumber { get; set; }

        // Foreign key relationship (1-N with Clinic)
        public int ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        // Relationships
        public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    }
}
