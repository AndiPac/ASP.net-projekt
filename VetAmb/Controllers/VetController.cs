using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class VetController : Controller
    {
        private readonly IVetRepository    _vetRepository;
        private readonly IClinicRepository _clinicRepository;

        public VetController(
            IVetRepository    vetRepository,
            IClinicRepository clinicRepository)
        {
            _vetRepository    = vetRepository;
            _clinicRepository = clinicRepository;
        }

        // ── LIST ──────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("vets")]
        public IActionResult Index()
        {
            var vets = _vetRepository.GetAll();
            return View(vets);
        }

        // ── DETAILS ────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("vets/{id:int}")]
        public IActionResult Details(int id)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
                return NotFound();
            return View(vet);
        }

        // ── CREATE ───────────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("vets/create")]
        public IActionResult Create()
        {
            return View(new VetFormModel());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("vets/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VetFormModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var vet = new Vet
            {
                FirstName          = model.FirstName,
                LastName           = model.LastName,
                Specialization     = model.Specialization,
                LicenseNumber      = model.LicenseNumber,
                YearsOfExperience  = model.YearsOfExperience,
                Phone              = model.Phone,
                HourlyRate         = model.HourlyRate,
                ClinicId           = model.ClinicId
            };

            _vetRepository.Add(vet);
            return RedirectToAction(nameof(Details), new { id = vet.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("vets/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
                return NotFound();

            var model = new VetFormModel
            {
                Id                = vet.Id,
                FirstName         = vet.FirstName,
                LastName          = vet.LastName,
                Specialization    = vet.Specialization,
                LicenseNumber     = vet.LicenseNumber,
                YearsOfExperience = vet.YearsOfExperience,
                Phone             = vet.Phone,
                HourlyRate        = vet.HourlyRate,
                ClinicId          = vet.ClinicId
            };

            // Pre-populate autocomplete display text for Clinic field.
            ViewBag.InitialClinicText        = vet.Clinic?.Name ?? "";
            // Pre-populate Specialization autocomplete display text.
            ViewBag.InitialSpecializationText = vet.Specialization.GetDisplayName();

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("vets/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, VetFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            // Fetch the tracked entity — never touch Id or DeletedAt.
            var vet = _vetRepository.GetById(id);
            if (vet == null)
                return NotFound();

            vet.FirstName         = model.FirstName;
            vet.LastName          = model.LastName;
            vet.Specialization    = model.Specialization;
            vet.LicenseNumber     = model.LicenseNumber;
            vet.YearsOfExperience = model.YearsOfExperience;
            vet.Phone             = model.Phone;
            vet.HourlyRate        = model.HourlyRate;
            vet.ClinicId          = model.ClinicId;

            _vetRepository.Update(vet);
            return RedirectToAction(nameof(Details), new { id = vet.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("vets/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _vetRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        // ── AJAX SEARCH ENDPOINTS ─────────────────────────────────

        /// <summary>
        /// Returns VeterinarySpecialization enum values matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the Specialization autocomplete field in Vet Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("vets/search-specialization")]
        public IActionResult SearchSpecialization(string? term)
        {
            var results = Enum.GetValues<VeterinarySpecialization>()
                .Select(s => new { id = (int)s, text = s.GetDisplayName() })
                .Where(x => string.IsNullOrWhiteSpace(term)
                       || x.text.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.text)
                .ToList();

            return Json(results);
        }

        /// <summary>
        /// Returns clinics matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the ClinicId autocomplete field in Vet Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("vets/search-clinics")]
        public IActionResult SearchClinics(string? term)
        {
            var results = _clinicRepository.GetAll()
                .Where(c => string.IsNullOrWhiteSpace(term)
                       || (c.Name ?? "").Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Name)
                .Take(10)
                .Select(c => new { id = c.Id, text = c.Name ?? "" })
                .ToList();

            return Json(results);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Resolves and stores display text for the Clinic autocomplete input so it
        /// can pre-fill correctly when a form is returned on validation failure.
        /// </summary>
        private void PopulateAutocompleteState(VetFormModel model)
        {
            if (model.ClinicId > 0)
            {
                var c = _clinicRepository.GetById(model.ClinicId);
                ViewBag.InitialClinicText = c?.Name ?? "";
            }
            ViewBag.InitialSpecializationText = model.Specialization.GetDisplayName();
        }
    }
}
