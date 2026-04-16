using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAll();
        Appointment? GetById(int id);
    }
}
