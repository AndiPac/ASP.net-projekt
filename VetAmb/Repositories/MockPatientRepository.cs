using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockPatientRepository : IPatientRepository
    {
        private readonly List<Patient> _patients;

        public MockPatientRepository()
        {
            _patients = SeedData.Patients;
        }

        public List<Patient> GetAll() => _patients;

        public List<Patient> Search(string term)
        {
            return _patients
                .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                         || p.Breed.Contains(term, StringComparison.OrdinalIgnoreCase)
                         || p.MicrochipId.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Patient? GetById(int id) => _patients.FirstOrDefault(p => p.Id == id);

        public void Add(Patient patient)
        {
            patient.Id = _patients.Count > 0 ? _patients.Max(p => p.Id) + 1 : 1;
            _patients.Add(patient);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(Patient patient) { }

        public void SoftDelete(int id)
        {
            var patient = _patients.FirstOrDefault(p => p.Id == id);
            if (patient != null) patient.DeletedAt = DateTime.UtcNow;
        }
    }
}
