using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public IActionResult Index()
        {
            var patients = _patientRepository.GetAll();
            return View(patients);
        }

        public IActionResult Details(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
                return NotFound();
            return View(patient);
        }
    }
}
