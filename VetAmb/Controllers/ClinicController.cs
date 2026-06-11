using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class ClinicController : Controller
    {
        private readonly IClinicRepository _clinicRepository;

        public ClinicController(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        // ── LIST ──────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("clinics")]
        public IActionResult Index()
        {
            var clinics = _clinicRepository.GetAll();
            return View(clinics);
        }

        // ── DETAILS ────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("clinics/{id:int}")]
        public IActionResult Details(int id)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
                return NotFound();
            return View(clinic);
        }

        // ── CREATE ───────────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("clinics/create")]
        public IActionResult Create()
        {
            return View(new ClinicFormModel());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("clinics/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ClinicFormModel model)
        {
            var parsedDate = ParseDate(model.FoundationDate);
            if (parsedDate == default)
                ModelState.AddModelError(nameof(model.FoundationDate), "Datum osnutka je obavezan.");

            if (!ModelState.IsValid)
                return View(model);

            var clinic = new Clinic
            {
                Name               = model.Name,
                Address            = model.Address,
                Phone              = model.Phone,
                Email              = model.Email,
                RegistrationNumber = model.RegistrationNumber,
                FoundationDate     = parsedDate,
                MaxCapacity        = model.MaxCapacity
            };

            _clinicRepository.Add(clinic);
            return RedirectToAction(nameof(Details), new { id = clinic.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("clinics/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
                return NotFound();

            var model = new ClinicFormModel
            {
                Id                 = clinic.Id,
                Name               = clinic.Name,
                Address            = clinic.Address,
                Phone              = clinic.Phone,
                Email              = clinic.Email,
                RegistrationNumber = clinic.RegistrationNumber,
                MaxCapacity        = clinic.MaxCapacity
            };

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("clinics/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ClinicFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // Fetch the tracked entity — never touch Id, DeletedAt, FoundationDate or MaxCapacity.
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
                return NotFound();

            clinic.Name               = model.Name;
            clinic.Address            = model.Address;
            clinic.Phone              = model.Phone;
            clinic.Email              = model.Email;
            clinic.RegistrationNumber = model.RegistrationNumber;
            clinic.MaxCapacity        = model.MaxCapacity;

            _clinicRepository.Update(clinic);
            return RedirectToAction(nameof(Details), new { id = clinic.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("clinics/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _clinicRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        // ── HELPERS ───────────────────────────────────────────────

        private static DateTime ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            DateTime.TryParseExact(
                value.Trim(),
                new[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "MM/dd/yyyy", "MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var result);
            return result;
        }
    }
}
