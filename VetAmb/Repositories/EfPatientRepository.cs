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
                    .ThenInclude(a => a.Vet)
                .Include(p => p.MedicalRecords)
                .ToList();
        }

        public List<Patient> Search(string term)
        {
            term ??= string.Empty;

            return _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Vet)
                .Include(p => p.MedicalRecords)
                .Where(p => (p.Name ?? string.Empty).Contains(term)
                         || (p.Breed ?? string.Empty).Contains(term)
                         || (p.MicrochipId ?? string.Empty).Contains(term))
                .ToList();
        }

        public Patient? GetById(int id)
        {
            return _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Vet)
                .Include(p => p.MedicalRecords)
                .FirstOrDefault(p => p.Id == id);
        }

        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void Update(Patient patient)
        {
            // Entity must already be tracked (loaded via GetById before mutation).
            _context.SaveChanges();
        }

        /// <summary>
        /// Marks a patient as deleted by setting DeletedAt. Never calls Remove().
        /// The global EF query filter (p => p.DeletedAt == null) hides it from all queries.
        /// </summary>
        public void SoftDelete(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null) return;
            patient.DeletedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
    }
}
