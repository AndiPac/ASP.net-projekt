using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IPatientRepository
    {
        List<Patient> GetAll();
        Patient? GetById(int id);
        void Add(Patient patient);
        void Update(Patient patient);
        void SoftDelete(int id);
    }
}
