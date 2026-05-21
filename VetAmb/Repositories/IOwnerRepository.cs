using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAll();
        Owner? GetById(int id);
        void Add(Owner owner);
        void Update(Owner owner);
        void SoftDelete(int id);
    }
}
