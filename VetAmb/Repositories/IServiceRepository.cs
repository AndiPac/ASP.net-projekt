using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetAll();
        Service? GetById(int id);
        void Add(Service service);
        void Update(Service service);
        void SoftDelete(int id);
    }
}
