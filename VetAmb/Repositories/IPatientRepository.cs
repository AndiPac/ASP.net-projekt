using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IPatientRepository
    {
        List<Patient> GetAll();
        Patient? GetById(int id);
    }
}
