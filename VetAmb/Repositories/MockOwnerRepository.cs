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

        public Owner? GetById(int id) => _owners.FirstOrDefault(o => o.Id == id);
    }
}
