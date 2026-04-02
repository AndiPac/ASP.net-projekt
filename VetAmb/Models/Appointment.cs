using System;
using System.Collections.Generic;

#nullable enable

namespace AspNetProjekt.Models
{
    /// <summary>
    /// Appointment class - represents a veterinary appointment (5 attributes)
    /// </summary>
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Reason { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }

        // Foreign key relationships
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int VetId { get; set; }
        public Vet? Vet { get; set; }

        // N-N Relationship with Service through AppointmentService junction table
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
