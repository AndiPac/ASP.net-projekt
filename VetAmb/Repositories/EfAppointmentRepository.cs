using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfAppointmentRepository : IAppointmentRepository
    {
        private readonly VetAmbDbContext _context;

        public EfAppointmentRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Appointment> GetAll()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Vet)
                    .ThenInclude(v => v!.Clinic)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(a => a.Service)
                .ToList();
        }

        public Appointment? GetById(int id)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Vet)
                    .ThenInclude(v => v!.Clinic)
                .Include(a => a.AppointmentServices)
                    .ThenInclude(a => a.Service)
                .FirstOrDefault(a => a.Id == id);
        }
    }
}
