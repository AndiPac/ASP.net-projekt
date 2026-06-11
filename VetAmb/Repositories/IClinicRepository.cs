using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IClinicRepository
    {
        List<Clinic> GetAll();
        List<Clinic> Search(string term);
        Clinic? GetById(int id);
        void Add(Clinic clinic);
        void Update(Clinic clinic);
        void SoftDelete(int id);
    }
}
