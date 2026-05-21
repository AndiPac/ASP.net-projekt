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

        public void Add(MedicalRecord record)
        {
            record.Id = _records.Count > 0 ? _records.Max(r => r.Id) + 1 : 1;
            _records.Add(record);
        }

        // In-memory: entity is already the same reference, nothing to persist.
        public void Update(MedicalRecord record) { }

        public void SoftDelete(int id)
        {
            var record = _records.FirstOrDefault(r => r.Id == id);
            if (record != null) record.DeletedAt = DateTime.UtcNow;
        }
    }
}
