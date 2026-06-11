using System;

#nullable enable

namespace VetAmb.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }
}
