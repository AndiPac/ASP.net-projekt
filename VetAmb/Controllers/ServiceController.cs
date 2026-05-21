using Microsoft.AspNetCore.Mvc;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // ── LIST ──────────────────────────────────────────────────────

        [Route("services")]
        public IActionResult Index()
        {
            var services = _serviceRepository.GetAll();
            return View(services);
        }

        // ── DETAILS ────────────────────────────────────────────────

        [Route("services/{id:int}")]
        public IActionResult Details(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
                return NotFound();
            return View(service);
        }

        // ── CREATE ───────────────────────────────────────────────

        [HttpGet]
        [Route("services/create")]
        public IActionResult Create()
        {
            return View(new ServiceFormModel());
        }

        [HttpPost]
        [Route("services/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var service = new Service
            {
                Name                     = model.Name,
                Description              = model.Description,
                Price                    = model.Price,
                EstimatedDurationMinutes = model.EstimatedDurationMinutes
            };

            _serviceRepository.Add(service);
            return RedirectToAction(nameof(Details), new { id = service.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────

        [HttpGet]
        [Route("services/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
                return NotFound();

            var model = new ServiceFormModel
            {
                Id                       = service.Id,
                Name                     = service.Name,
                Description              = service.Description,
                Price                    = service.Price,
                EstimatedDurationMinutes = service.EstimatedDurationMinutes
            };

            return View(model);
        }

        [HttpPost]
        [Route("services/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ServiceFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // Fetch the tracked entity — never touch Id or DeletedAt.
            var service = _serviceRepository.GetById(id);
            if (service == null)
                return NotFound();

            service.Name                     = model.Name;
            service.Description              = model.Description;
            service.Price                    = model.Price;
            service.EstimatedDurationMinutes = model.EstimatedDurationMinutes;

            _serviceRepository.Update(service);
            return RedirectToAction(nameof(Details), new { id = service.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────

        [HttpPost]
        [Route("services/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _serviceRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
