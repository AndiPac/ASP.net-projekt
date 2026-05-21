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

        public void Add(Clinic clinic)
        {
            _context.Clinics.Add(clinic);
            _context.SaveChanges();
        }

        public void Update(Clinic clinic)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks a clinic as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (c => c.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var clinic = _context.Clinics.Find(id);
            if (clinic == null) return;
            clinic.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
