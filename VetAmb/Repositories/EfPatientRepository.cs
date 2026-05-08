using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class EfPatientRepository : IPatientRepository
    {
        private readonly VetAmbDbContext _context;

        public EfPatientRepository(VetAmbDbContext context)
        {
            _context = context;
        }

        public List<Patient> GetAll()
        {
            return _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                .Include(p => p.MedicalRecords)
                .ToList();
        }

        public Patient? GetById(int id)
        {
            return _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                .Include(p => p.MedicalRecords)
                .FirstOrDefault(p => p.Id == id);
        }
    }
}
