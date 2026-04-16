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

        public Service? GetById(int id) => _services.FirstOrDefault(s => s.Id == id);
    }
}
