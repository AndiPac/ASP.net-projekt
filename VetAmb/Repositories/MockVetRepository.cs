using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockVetRepository : IVetRepository
    {
        private readonly List<Vet> _vets;

        public MockVetRepository()
        {
            _vets = SeedData.Vets;
        }

        public List<Vet> GetAll() => _vets;

        public Vet? GetById(int id) => _vets.FirstOrDefault(v => v.Id == id);
    }
}
