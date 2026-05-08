using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfClinicRepository : IClinicRepository
    {
        private readonly VetAmbDbContext _context;

        public EfClinicRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Clinic> GetAll()
        {
            return _context.Clinics
                .Include(c => c.Vets)
                .Include(c => c.Owners)
                .ToList();
        }

        public Clinic? GetById(int id)
        {
            return _context.Clinics
                .Include(c => c.Vets)
                .Include(c => c.Owners)
                .FirstOrDefault(c => c.Id == id);
        }
    }
}
