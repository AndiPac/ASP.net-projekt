using System.Collections.Generic;

#nullable enable

namespace AspNetProjekt.Models
{
    /// <summary>
    /// Service class - represents a veterinary service (5 attributes)
    /// </summary>
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDurationMinutes { get; set; }

        // N-N Relationship with Appointment through AppointmentService junction table
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}

