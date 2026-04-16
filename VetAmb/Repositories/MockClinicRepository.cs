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
    }
}
