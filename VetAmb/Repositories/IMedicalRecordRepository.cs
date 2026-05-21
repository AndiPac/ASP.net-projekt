using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecord> GetAll();
        MedicalRecord? GetById(int id);
        void Add(MedicalRecord record);
        void Update(MedicalRecord record);
        void SoftDelete(int id);
    }
}
