using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Patient class - represents a pet (Complex class with 8 attributes)
    /// </summary>
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public AnimalSpecies Species { get; set; }
        public string? Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string? MicrochipId { get; set; }
        public string? Color { get; set; }

        // Foreign key relationship (1-N with Owner)
        [ForeignKey(nameof(Owner))]
        public int OwnerId { get; set; }
        public virtual Owner? Owner { get; set; }

        // Relationships
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
