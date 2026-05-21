using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments;

        public MockAppointmentRepository()
        {
            _appointments = SeedData.Appointments;
            ResolvePassedAppointments();
        }

        public List<Appointment> GetAll() => _appointments;

        public Appointment? GetById(int id) => _appointments.FirstOrDefault(a => a.Id == id);

        public void Add(Appointment appointment)
        {
            appointment.Id = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
            _appointments.Add(appointment);
        }

        public void Update(Appointment appointment) { /* in-memory: entity already mutated */ }

        public void SoftDelete(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
                appointment.DeletedAt = DateTime.UtcNow;
        }
        private void ResolvePassedAppointments()
        {
            foreach (var appt in _appointments)
            {
                if (appt.AppointmentDateTime < DateTime.Now &&
                    appt.Status == AppointmentStatus.Scheduled)
                {
                    appt.Status = AppointmentStatus.Completed;
                }
            }
        }
    }
}
