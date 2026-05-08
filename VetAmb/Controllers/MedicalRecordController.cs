using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordController(IMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        [Route("medical-records")]
        public IActionResult Index()
        {
            var records = _medicalRecordRepository.GetAll();
            return View(records);
        }

        [Route("medical-records/{id}")]
        public IActionResult Details(int id)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
                return NotFound();
            return View(record);
        }
    }
}
