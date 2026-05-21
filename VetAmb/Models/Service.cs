using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Service class - represents a veterinary service (5 attributes)
    /// </summary>
    public class Service
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDurationMinutes { get; set; }

        // Soft delete — never physically remove service records.
        public DateTime? DeletedAt { get; set; }

        // N-N Relationship with Appointment through AppointmentService junction table
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}

