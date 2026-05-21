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

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        /// <summary>
        /// Persists changes to a tracked Appointment entity.
        /// The entity must already be fetched (and therefore tracked) within the
        /// same request scope before calling this method.
        /// </summary>
        public void Update(Appointment appointment)
        {
            // Entity is already tracked from GetById — SaveChanges picks up the diff.
            _context.SaveChanges();
        }

        /// <summary>
        /// Soft-delete: sets DeletedAt timestamp instead of calling Remove().
        /// The global query filter on Appointment will exclude this record from all
        /// future queries automatically.
        /// </summary>
        public void SoftDelete(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                appointment.DeletedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }
        }
    }
}
