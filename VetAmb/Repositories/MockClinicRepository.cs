using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockClinicRepository : IClinicRepository
    {
        private readonly List<Clinic> _clinics;

        public MockClinicRepository()
        {
            _clinics = SeedData.Clinics;
        }

        public List<Clinic> GetAll() => _clinics;

        public Clinic? GetById(int id) => _clinics.FirstOrDefault(c => c.Id == id);

        public void Add(Clinic clinic)
        {
            clinic.Id = _clinics.Count > 0 ? _clinics.Max(c => c.Id) + 1 : 1;
            _clinics.Add(clinic);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(Clinic clinic) { }

        public void SoftDelete(int id)
        {
            var clinic = _clinics.FirstOrDefault(c => c.Id == id);
            if (clinic != null) clinic.DeletedAt = DateTime.UtcNow;
        }
    }
}
