using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfMedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly VetAmbDbContext _context;

        public EfMedicalRecordRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<MedicalRecord> GetAll()
        {
            return _context.MedicalRecords
                .Include(r => r.Patient)
                .ToList();
        }

        public MedicalRecord? GetById(int id)
        {
            return _context.MedicalRecords
                .Include(r => r.Patient)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Add(MedicalRecord record)
        {
            _context.MedicalRecords.Add(record);
            _context.SaveChanges();
        }

        public void Update(MedicalRecord record)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks a record as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (m => m.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var record = _context.MedicalRecords.Find(id);
            if (record == null) return;
            record.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
