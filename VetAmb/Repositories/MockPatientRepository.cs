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

        public Patient? GetById(int id) => _patients.FirstOrDefault(p => p.Id == id);
    }
}
