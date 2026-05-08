using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Appointment class - represents a veterinary appointment (5 attributes)
    /// </summary>
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Reason { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? RescheduleReason { get; set; }

        // Foreign key relationships
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey(nameof(Vet))]
        public int VetId { get; set; }
        public virtual Vet? Vet { get; set; }

        // N-N Relationship with Service through AppointmentService junction table
        public virtual ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
    }
}
