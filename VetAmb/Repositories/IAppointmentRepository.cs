using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAll();
        Appointment? GetById(int id);
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void SoftDelete(int id);
    }
}
