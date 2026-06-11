using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class VetDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Specialization { get; set; }
        public string? LicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Phone { get; set; }
        public decimal HourlyRate { get; set; }
        public int ClinicId { get; set; }
        public ClinicDTO? Clinic { get; set; }
    }
}
