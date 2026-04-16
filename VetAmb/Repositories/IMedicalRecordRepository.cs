using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecord> GetAll();
        MedicalRecord? GetById(int id);
    }
}
