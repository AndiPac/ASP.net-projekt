using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class OwnerDTO
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? IdNumber { get; set; }
        public int ClinicId { get; set; }
        public ClinicDTO? Clinic { get; set; }
    }
}
