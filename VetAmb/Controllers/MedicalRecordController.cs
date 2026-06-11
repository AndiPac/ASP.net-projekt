using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository       _patientRepository;

        public MedicalRecordController(
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository       patientRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository       = patientRepository;
        }

        // ── LIST ──────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("medical-records")]
        public IActionResult Index()
        {
            var records = _medicalRecordRepository.GetAll();
            return View(records);
        }

        // ── DETAILS ────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("medical-records/{id:int}")]
        public IActionResult Details(int id)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
                return NotFound();
            return View(record);
        }

        // ── CREATE ───────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("medical-records/create")]
        public IActionResult Create()
        {
            return View(new MedicalRecordFormModel());
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("medical-records/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalRecordFormModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var recordDate = ParseDate(model.DateRecorded);
            if (recordDate == null)
            {
                ModelState.AddModelError(nameof(model.DateRecorded), "Neispravan format datuma. Koristite dd.MM.yyyy HH:mm.");
                PopulateAutocompleteState(model);
                return View(model);
            }

            var record = new MedicalRecord
            {
                Diagnosis  = model.Diagnosis,
                Treatment  = model.Treatment,
                Notes      = model.Notes,
                RecordDate = recordDate.Value,
                PatientId  = model.PatientId
            };

            _medicalRecordRepository.Add(record);
            return RedirectToAction(nameof(Details), new { id = record.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("medical-records/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
                return NotFound();

            var model = new MedicalRecordFormModel
            {
                Id           = record.Id,
                DateRecorded = record.RecordDate.ToString(
                    Request.Headers.AcceptLanguage.FirstOrDefault()
                        ?.Split(',', ';')[0].Trim()
                        .StartsWith("hr", StringComparison.OrdinalIgnoreCase) ?? true
                        ? "dd.MM.yyyy HH:mm"
                        : "MM/dd/yyyy hh:mm tt",
                    System.Globalization.CultureInfo.InvariantCulture),
                Diagnosis    = record.Diagnosis,
                Treatment    = record.Treatment,
                Notes        = record.Notes,
                PatientId    = record.PatientId
            };

            // Pre-populate autocomplete display text for Patient field.
            ViewBag.InitialPatientText = record.Patient?.Name ?? "";

            return View(model);
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("medical-records/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, MedicalRecordFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var recordDate = ParseDate(model.DateRecorded);
            if (recordDate == null)
            {
                ModelState.AddModelError(nameof(model.DateRecorded), "Neispravan format datuma. Koristite dd.MM.yyyy HH:mm.");
                PopulateAutocompleteState(model);
                return View(model);
            }

            // Fetch the tracked entity — never touch Id or DeletedAt.
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
                return NotFound();

            record.Diagnosis  = model.Diagnosis;
            record.Treatment  = model.Treatment;
            record.Notes      = model.Notes;
            record.RecordDate = recordDate.Value;
            record.PatientId  = model.PatientId;

            _medicalRecordRepository.Update(record);
            return RedirectToAction(nameof(Details), new { id = record.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("medical-records/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _medicalRecordRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        // ── AJAX SEARCH ENDPOINTS ─────────────────────────────────

        /// <summary>
        /// Returns patients matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the PatientId autocomplete field in MedicalRecord Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("medicalrecords/search-patients")]
        public IActionResult SearchPatients(string? term)
        {
            var results = _patientRepository.GetAll()
                .Where(p => string.IsNullOrWhiteSpace(term)
                       || (p.Name ?? "").Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Name)
                .Take(10)
                .Select(p => new { id = p.Id, text = p.Name ?? "" })
                .ToList();

            return Json(results);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Resolves and stores display text for the Patient autocomplete input so it
        /// can pre-fill correctly when a form is returned on validation failure.
        /// </summary>
        private void PopulateAutocompleteState(MedicalRecordFormModel model)
        {
            if (model.PatientId > 0)
            {
                var p = _patientRepository.GetById(model.PatientId);
                ViewBag.InitialPatientText = p?.Name ?? "";
            }
        }

        /// <summary>
        /// Parses a date string emitted by _CustomDateTimePicker.
        /// Accepted formats: dd.MM.yyyy HH:mm (hr), dd.MM.yyyy,
        ///                   MM/dd/yyyy hh:mm tt (en-US), MM/dd/yyyy, MM/dd/yyyy HH:mm.
        /// </summary>
        private static DateTime? ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string[] formats =
            [
                "dd.MM.yyyy HH:mm",
                "dd.MM.yyyy",
                "MM/dd/yyyy hh:mm tt",
                "MM/dd/yyyy HH:mm",
                "MM/dd/yyyy"
            ];

            if (DateTime.TryParseExact(
                    raw.Trim(),
                    formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var result))
                return result;

            return null;
        }
    }
}
