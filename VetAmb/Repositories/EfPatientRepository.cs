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

        public List<Patient> Search(string term)
        {
            return _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                .Include(p => p.MedicalRecords)
                .Where(p => p.Name.Contains(term)
                         || p.Breed.Contains(term)
                         || p.MicrochipId.Contains(term))
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
