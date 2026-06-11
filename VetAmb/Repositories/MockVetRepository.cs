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

        public List<Vet> Search(string term)
        {
            return _vets
                .Where(v => (v.FirstName ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (v.LastName ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (v.LicenseNumber ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (v.Phone ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Vet? GetById(int id) => _vets.FirstOrDefault(v => v.Id == id);

        public void Add(Vet vet)
        {
            vet.Id = _vets.Count > 0 ? _vets.Max(v => v.Id) + 1 : 1;
            _vets.Add(vet);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(Vet vet) { }

        public void SoftDelete(int id)
        {
            var vet = _vets.FirstOrDefault(v => v.Id == id);
            if (vet != null) vet.DeletedAt = DateTime.UtcNow;
        }
    }
}
