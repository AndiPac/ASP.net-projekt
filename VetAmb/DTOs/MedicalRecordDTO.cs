using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class MedicalRecordDTO
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public DateTime RecordDate { get; set; }
        public string? Notes { get; set; }
        public int PatientId { get; set; }
        public PatientDTO? Patient { get; set; }
    }
}
