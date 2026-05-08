#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetAmb.Models
{
    /// <summary>
    /// AppointmentService class - junction table for N-N relationship between Appointment and Service
    /// </summary>
    public class AppointmentService
    {
        [Key]
        public int Id { get; set; }

        // Foreign keys
        [ForeignKey(nameof(Appointment))]
        public int AppointmentId { get; set; }
        public virtual Appointment? Appointment { get; set; }

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }
    }
}
