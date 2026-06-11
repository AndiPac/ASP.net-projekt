using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class ClinicDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime FoundationDate { get; set; }
        public int MaxCapacity { get; set; }
        public string? RegistrationNumber { get; set; }
    }
}
