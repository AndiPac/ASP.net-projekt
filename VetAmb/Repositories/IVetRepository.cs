using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IVetRepository
    {
        List<Vet> GetAll();
        Vet? GetById(int id);
        void Add(Vet vet);
        void Update(Vet vet);
        void SoftDelete(int id);
    }
}
