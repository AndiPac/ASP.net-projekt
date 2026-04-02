#nullable enable

namespace AspNetProjekt.Models
{
    /// <summary>
    /// AppointmentService class - junction table for N-N relationship between Appointment and Service
    /// </summary>
    public class AppointmentService
    {
        public int Id { get; set; }

        // Foreign keys
        public int AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }
}
