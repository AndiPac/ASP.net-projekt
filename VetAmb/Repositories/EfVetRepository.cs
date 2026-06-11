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

        public List<Vet> Search(string term)
        {
            return _context.Vets
                .Include(v => v.Clinic)
                .Where(v => (v.FirstName ?? string.Empty).Contains(term)
                         || (v.LastName ?? string.Empty).Contains(term)
                         || (v.LicenseNumber ?? string.Empty).Contains(term)
                         || (v.Phone ?? string.Empty).Contains(term))
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

        public void Add(Vet vet)
        {
            _context.Vets.Add(vet);
            _context.SaveChanges();
        }

        public void Update(Vet vet)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks a vet as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (v => v.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var vet = _context.Vets.Find(id);
            if (vet == null) return;
            vet.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
