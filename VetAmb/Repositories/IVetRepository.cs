using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IVetRepository
    {
        List<Vet> GetAll();
        Vet? GetById(int id);
    }
}
