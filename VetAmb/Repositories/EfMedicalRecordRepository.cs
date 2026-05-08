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
    }
}
