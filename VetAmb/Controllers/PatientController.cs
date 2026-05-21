using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IOwnerRepository   _ownerRepository;

        public PatientController(
            IPatientRepository patientRepository,
            IOwnerRepository   ownerRepository)
        {
            _patientRepository = patientRepository;
            _ownerRepository   = ownerRepository;
        }

        // ── LIST ─────────────────────────────────────────────────────────

        [Route("patients")]
        public IActionResult Index()
        {
            var patients = _patientRepository.GetAll();
            return View(patients);
        }

        // ── DETAILS ──────────────────────────────────────────────────────

        [Route("patients/{id:int}")]
        public IActionResult Details(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
                return NotFound();
            return View(patient);
        }

        // ── CREATE ───────────────────────────────────────────────────────

        [HttpGet]
        [Route("patients/create")]
        public IActionResult Create()
        {
            return View(new PatientFormModel());
        }

        [HttpPost]
        [Route("patients/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientFormModel model)
        {
            var parsedDob = ParseDate(model.DateOfBirth);
            if (parsedDob == default)
                ModelState.AddModelError(nameof(model.DateOfBirth),
                    "Unesite ispravan datum (npr. 15.03.1990).");

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var patient = new Patient
            {
                Name        = model.Name,
                Species     = model.Species,
                Breed       = model.Breed,
                DateOfBirth = parsedDob,
                Weight      = model.Weight,
                MicrochipId = model.MicrochipId,
                Color       = model.Color,
                OwnerId     = model.OwnerId
            };

            _patientRepository.Add(patient);
            return RedirectToAction(nameof(Details), new { id = patient.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────────────

        [HttpGet]
        [Route("patients/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
                return NotFound();

            var model = new PatientFormModel
            {
                Id          = patient.Id,
                Name        = patient.Name,
                Species     = patient.Species,
                Breed       = patient.Breed,
                DateOfBirth = patient.DateOfBirth.ToString(
                    Request.Headers.AcceptLanguage.FirstOrDefault()
                        ?.Split(',', ';')[0].Trim()
                        .StartsWith("hr", StringComparison.OrdinalIgnoreCase) ?? true
                        ? "dd.MM.yyyy"
                        : "MM/dd/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture),
                Weight      = patient.Weight,
                MicrochipId = patient.MicrochipId,
                Color       = patient.Color,
                OwnerId     = patient.OwnerId
            };

            // Pass the original date string for the datepicker partial to pre-populate.
            ViewData["PickerValue"]    = patient.DateOfBirth;
            // Pre-populate autocomplete display text for Owner field.
            ViewBag.InitialOwnerText  = patient.Owner != null
                ? $"{patient.Owner.FirstName} {patient.Owner.LastName}"
                : "";
            // Pre-populate Species autocomplete display text.
            ViewBag.InitialSpeciesText = patient.Species.GetDisplayName();

            return View(model);
        }

        [HttpPost]
        [Route("patients/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var parsedDob = ParseDate(model.DateOfBirth);
            if (parsedDob == default)
                ModelState.AddModelError(nameof(model.DateOfBirth),
                    "Unesite ispravan datum (npr. 15.03.1990).");

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            // Fetch the tracked entity — never touch Id or DeletedAt.
            var patient = _patientRepository.GetById(id);
            if (patient == null)
                return NotFound();

            patient.Name        = model.Name;
            patient.Species     = model.Species;
            patient.Breed       = model.Breed;
            patient.DateOfBirth = parsedDob;
            patient.Weight      = model.Weight;
            patient.MicrochipId = model.MicrochipId;
            patient.Color       = model.Color;
            patient.OwnerId     = model.OwnerId;

            _patientRepository.Update(patient);
            return RedirectToAction(nameof(Details), new { id = patient.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────────────

        [HttpPost]
        [Route("patients/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _patientRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns AnimalSpecies enum values matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the Species autocomplete field in Patient Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("patients/search-species")]
        public IActionResult SearchSpecies(string? term)
        {
            var results = Enum.GetValues<AnimalSpecies>()
                .Select(s => new { id = (int)s, text = s.GetDisplayName() })
                .Where(x => string.IsNullOrWhiteSpace(term)
                       || x.text.Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.text)
                .ToList();

            return Json(results);
        }

        // ── AJAX SEARCH ENDPOINTS ─────────────────────────────────────────

        /// <summary>
        /// Returns active owners matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the OwnerId autocomplete field in Patient Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("patients/search-owners")]
        public IActionResult SearchOwners(string? term)
        {
            var results = _ownerRepository.GetAll()
                .Where(o => string.IsNullOrWhiteSpace(term)
                       || (o.FirstName ?? "").Contains(term, StringComparison.OrdinalIgnoreCase)
                       || (o.LastName  ?? "").Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.LastName)
                .Take(10)
                .Select(o => new { id = o.Id, text = $"{o.FirstName} {o.LastName}" })
                .ToList();

            return Json(results);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Resolves and stores display text for the Owner autocomplete input so it
        /// can pre-fill correctly when a form is returned on validation failure.
        /// </summary>
        private void PopulateAutocompleteState(PatientFormModel model)
        {
            if (model.OwnerId > 0)
            {
                var o = _ownerRepository.GetById(model.OwnerId);
                ViewBag.InitialOwnerText = o != null
                    ? $"{o.FirstName} {o.LastName}"
                    : "";
            }
        }

        /// <summary>
        /// Parses a date string from the custom datepicker output.
        /// Accepts both date-only and date+time variants so the picker's time spinners
        /// are tolerated even though DateOfBirth needs only the date part.
        /// Formats accepted: dd.MM.yyyy, dd.MM.yyyy HH:mm, MM/dd/yyyy, MM/dd/yyyy hh:mm tt
        /// </summary>
        private static DateTime ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            DateTime.TryParseExact(
                value,
                new[] { "dd.MM.yyyy", "dd.MM.yyyy HH:mm", "MM/dd/yyyy", "MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result);
            return result;
        }
    }
}

