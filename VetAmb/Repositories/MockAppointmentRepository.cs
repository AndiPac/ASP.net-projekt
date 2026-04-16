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
