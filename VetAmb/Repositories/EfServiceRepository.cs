using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfServiceRepository : IServiceRepository
    {
        private readonly VetAmbDbContext _context;

        public EfServiceRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Service> GetAll()
        {
            return _context.Services
                .Include(s => s.AppointmentServices)
                .ToList();
        }

        public List<Service> Search(string term)
        {
            return _context.Services
                .Include(s => s.AppointmentServices)
                .Where(s => (s.Name ?? string.Empty).Contains(term)
                         || (s.Description ?? string.Empty).Contains(term))
                .ToList();
        }

        public Service? GetById(int id)
        {
            return _context.Services
                .Include(s => s.AppointmentServices)
                    .ThenInclude(a => a.Appointment!)
                        .ThenInclude(a => a.Patient)
                .Include(s => s.AppointmentServices)
                    .ThenInclude(a => a.Appointment!)
                        .ThenInclude(a => a.Vet)
                .FirstOrDefault(s => s.Id == id);
        }

        public void Add(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
        }

        public void Update(Service service)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks a service as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (s => s.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null) return;
            service.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
