using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class PatientDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal Weight { get; set; }
        public string? MicrochipId { get; set; }
        public string? Color { get; set; }
        public int OwnerId { get; set; }
        public OwnerDTO? Owner { get; set; }
    }
}
