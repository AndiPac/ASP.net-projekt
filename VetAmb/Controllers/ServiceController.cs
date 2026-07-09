using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
        [Route("services")]
        public IActionResult Index()
        {
            var services = _serviceRepository.GetAll();
            return View(services);
        }

        // ── DETAILS ────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("services/{id:int}")]
        public IActionResult Details(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
                return RedirectToAction("StatusCodePage", "Error", new { statusCode = StatusCodes.Status404NotFound });
            return View(service);
        }

        // ── CREATE ───────────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("services/create")]
        public IActionResult Create()
        {
            return View(new ServiceFormModel());
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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
