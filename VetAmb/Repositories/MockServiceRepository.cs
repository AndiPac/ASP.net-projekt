using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockServiceRepository : IServiceRepository
    {
        private readonly List<Service> _services;

        public MockServiceRepository()
        {
            _services = SeedData.Services;
        }

        public List<Service> GetAll() => _services;

        public List<Service> Search(string term)
        {
            return _services
                .Where(s => (s.Name ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                         || (s.Description ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public Service? GetById(int id) => _services.FirstOrDefault(s => s.Id == id);

        public void Add(Service service)
        {
            service.Id = _services.Count > 0 ? _services.Max(s => s.Id) + 1 : 1;
            _services.Add(service);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(Service service) { }

        public void SoftDelete(int id)
        {
            var service = _services.FirstOrDefault(s => s.Id == id);
            if (service != null) service.DeletedAt = DateTime.UtcNow;
        }
    }
}
