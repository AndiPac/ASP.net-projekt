#nullable enable
using System;

namespace AspNetProjekt.Models
{
    /// <summary>
    /// MedicalRecord class - represents a patient's medical record (5 attributes)
    /// </summary>
    public class MedicalRecord
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }

        // Foreign key relationship (1-N with Patient)
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
