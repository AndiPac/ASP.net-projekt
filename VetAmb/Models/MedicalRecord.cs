#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetAmb.Models
{
    /// <summary>
    /// MedicalRecord class - represents a patient's medical record (5 attributes)
    /// </summary>
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }

        // Foreign key relationship (1-N with Patient)
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        // Soft delete — never call Remove(); set this timestamp instead.
        public DateTime? DeletedAt { get; set; }
    }
}
