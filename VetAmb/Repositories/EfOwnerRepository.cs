using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfOwnerRepository : IOwnerRepository
    {
        private readonly VetAmbDbContext _context;

        public EfOwnerRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Owner> GetAll()
        {
            return _context.Owners
                .Include(o => o.Clinic)
                .Include(o => o.Patients)
                .ToList();
        }

        public Owner? GetById(int id)
        {
            return _context.Owners
                .Include(o => o.Clinic)
                .Include(o => o.Patients)
                .FirstOrDefault(o => o.Id == id);
        }
    }
}
