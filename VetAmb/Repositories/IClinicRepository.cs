using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IClinicRepository
    {
        List<Clinic> GetAll();
        Clinic? GetById(int id);
    }
}
