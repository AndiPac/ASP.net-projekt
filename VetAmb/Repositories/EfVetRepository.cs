using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfVetRepository : IVetRepository
    {
        private readonly VetAmbDbContext _context;

        public EfVetRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Vet> GetAll()
        {
            return _context.Vets
                .Include(v => v.Clinic)
                .ToList();
        }

        public Vet? GetById(int id)
        {
            return _context.Vets
                .Include(v => v.Clinic)
                .Include(v => v.Appointments)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefault(v => v.Id == id);
        }
    }
}
