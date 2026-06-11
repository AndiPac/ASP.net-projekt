using VetAmb.Models;

namespace VetAmb.Repositories
{
    public interface IMedicalRecordRepository
    {
        List<MedicalRecord> GetAll();
        List<MedicalRecord> Search(string term);
        MedicalRecord? GetById(int id);
        void Add(MedicalRecord record);
        void Update(MedicalRecord record);
        void SoftDelete(int id);
    }
}
