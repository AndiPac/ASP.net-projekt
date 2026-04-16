using VetAmb.Models;

namespace VetAmb.Repositories
{
    public class MockMedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly List<MedicalRecord> _records;

        public MockMedicalRecordRepository()
        {
            _records = SeedData.MedicalRecords;
        }

        public List<MedicalRecord> GetAll() => _records;

        public MedicalRecord? GetById(int id) => _records.FirstOrDefault(r => r.Id == id);
    }
}
