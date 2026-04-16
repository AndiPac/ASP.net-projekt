using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IOwnerRepository
    {
        List<Owner> GetAll();
        Owner? GetById(int id);
    }
}
