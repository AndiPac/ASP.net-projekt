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

        public void Add(Owner owner)
        {
            _context.Owners.Add(owner);
            _context.SaveChanges();
        }

        public void Update(Owner owner)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks an owner as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (o => o.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var owner = _context.Owners.Find(id);
            if (owner == null) return;
            owner.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
