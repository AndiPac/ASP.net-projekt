using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IVetRepository
    {
        List<Vet> GetAll();
        List<Vet> Search(string term);
        Vet? GetById(int id);
        void Add(Vet vet);
        void Update(Vet vet);
        void SoftDelete(int id);
    }
}
