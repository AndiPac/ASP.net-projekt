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
    }
}
