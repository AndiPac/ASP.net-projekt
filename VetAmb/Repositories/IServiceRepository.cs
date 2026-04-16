using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetAll();
        Service? GetById(int id);
    }
}
