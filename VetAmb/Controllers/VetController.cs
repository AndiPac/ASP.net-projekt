using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class VetController : Controller
    {
        private readonly IVetRepository _vetRepository;

        public VetController(IVetRepository vetRepository)
        {
            _vetRepository = vetRepository;
        }

        public IActionResult Index()
        {
            var vets = _vetRepository.GetAll();
            return View(vets);
        }

        public IActionResult Details(int id)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
                return NotFound();
            return View(vet);
        }
    }
}
