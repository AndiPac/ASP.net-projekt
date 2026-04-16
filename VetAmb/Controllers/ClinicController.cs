using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IClinicRepository _clinicRepository;

        public ClinicController(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public IActionResult Index()
        {
            var clinics = _clinicRepository.GetAll();
            return View(clinics);
        }

        public IActionResult Details(int id)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
                return NotFound();
            return View(clinic);
        }
    }
}
