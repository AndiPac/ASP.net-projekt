using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockOwnerRepository : IOwnerRepository
    {
        private readonly List<Owner> _owners;

        public MockOwnerRepository()
        {
            _owners = SeedData.Owners;
        }

        public List<Owner> GetAll() => _owners;

        public List<Owner> Search(string term)
        {
            return _owners
                .Where(o => (o.FirstName ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (o.LastName ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (o.Email ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (o.Phone ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (o.IdNumber ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Owner? GetById(int id) => _owners.FirstOrDefault(o => o.Id == id);

        public void Add(Owner owner)
        {
            owner.Id = _owners.Count > 0 ? _owners.Max(o => o.Id) + 1 : 1;
            _owners.Add(owner);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(Owner owner) { }

        public void SoftDelete(int id)
        {
            var owner = _owners.FirstOrDefault(o => o.Id == id);
            if (owner != null) owner.DeletedAt = DateTime.UtcNow;
        }
    }
}
