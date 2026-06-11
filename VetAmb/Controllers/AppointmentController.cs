using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text.Json;
using VetAmb.Data;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository     _patientRepository;
        private readonly IVetRepository         _vetRepository;
        private readonly IServiceRepository     _serviceRepository;
        private readonly VetAmbDbContext        _context;

        public AppointmentController(
            IAppointmentRepository appointmentRepository,
            IPatientRepository     patientRepository,
            IVetRepository         vetRepository,
            IServiceRepository     serviceRepository,
            VetAmbDbContext        context)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository     = patientRepository;
            _vetRepository         = vetRepository;
            _serviceRepository     = serviceRepository;
            _context               = context;
        }

        // ── LIST ─────────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("appointments")]
        public IActionResult Index()
        {
            var appointments = _appointmentRepository.GetAll();
            return View(appointments);
        }

        // ── DETAILS ──────────────────────────────────────────────────────

        // {id:int} constraint ensures "appointments/create" never matches this route.
        [AllowAnonymous]
        [Route("appointments/{id:int}")]
        public IActionResult Details(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();
            return View(appointment);
        }

        // ── CREATE ───────────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("appointments/create")]
        public IActionResult Create()
        {
            ViewBag.SelectedServicesJson = "[]";
            return View(new AppointmentFormModel());
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("appointments/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentFormModel model)
        {
            var parsedDate = ParseDate(model.AppointmentDateTime);
            if (parsedDate == default)
                ModelState.AddModelError(
                    nameof(model.AppointmentDateTime),
                    "Neispravan format datuma. Koristite dd.MM.yyyy HH:mm.");

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var appointment = new Appointment
            {
                AppointmentDateTime = parsedDate,
                Reason              = model.Reason,
                Status              = model.Status,
                Notes               = model.Notes,
                RescheduleReason    = model.RescheduleReason,
                PatientId           = model.PatientId,
                VetId               = model.VetId
                // Id, DeletedAt — never set from form input
            };

            _appointmentRepository.Add(appointment);

            foreach (var sid in model.ServiceIds)
                _context.AppointmentServices.Add(new AppointmentService { AppointmentId = appointment.Id, ServiceId = sid });
            if (model.ServiceIds.Any())
                _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // ── EDIT ─────────────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("appointments/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();

            var model = new AppointmentFormModel
            {
                Id                  = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime.ToString(
                    System.Threading.Thread.CurrentThread.CurrentCulture.Name.StartsWith("hr")
                        ? "dd.MM.yyyy HH:mm"
                        : "MM/dd/yyyy hh:mm tt",
                    System.Globalization.CultureInfo.InvariantCulture),
                Reason              = appointment.Reason ?? "",
                Status              = appointment.Status,
                Notes               = appointment.Notes,
                RescheduleReason    = appointment.RescheduleReason,
                PatientId           = appointment.PatientId,
                VetId               = appointment.VetId,
                ServiceIds          = appointment.AppointmentServices.Select(s => s.ServiceId).ToList()
            };

            // Pass the original DateTime for the datepicker partial to pre-populate.
            ViewData["PickerValue"]     = appointment.AppointmentDateTime;
            // Pre-populate autocomplete display text for Patient and Vet fields.
            ViewBag.InitialPatientText = appointment.Patient?.Name ?? "";
            ViewBag.InitialVetText     = appointment.Vet != null
                ? $"Dr. {appointment.Vet.FirstName} {appointment.Vet.LastName}"
                : "";
            // Pass pre-selected services as JSON for Select2 to initialise tokens.
            var selSvcs = appointment.AppointmentServices
                .Where(s => s.Service != null)
                .Select(s => new { id = s.ServiceId, text = s.Service!.Name ?? "" })
                .ToList();
            ViewBag.SelectedServicesJson = JsonSerializer.Serialize(selSvcs);
            return View(model);
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("appointments/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AppointmentFormModel model)
        {
            var parsedDate = ParseDate(model.AppointmentDateTime);
            if (parsedDate == default)
                ModelState.AddModelError(
                    nameof(model.AppointmentDateTime),
                    "Neispravan format datuma. Koristite dd.MM.yyyy HH:mm.");

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            // Fetch the tracked entity — never touch Id, DeletedAt, or audit columns.
            var existing = _appointmentRepository.GetById(id);
            if (existing == null)
                return NotFound();

            // Explicit, safe field mapping only.
            existing.AppointmentDateTime = parsedDate;
            existing.Reason              = model.Reason;
            existing.Status              = model.Status;
            existing.Notes               = model.Notes;
            existing.RescheduleReason    = model.RescheduleReason;
            existing.PatientId           = model.PatientId;
            existing.VetId               = model.VetId;
            // existing.Id, existing.DeletedAt — intentionally not mapped.

            // Replace AppointmentService junction records.
            var oldSvcs = _context.AppointmentServices.Where(x => x.AppointmentId == id).ToList();
            _context.AppointmentServices.RemoveRange(oldSvcs);
            foreach (var sid in model.ServiceIds)
                _context.AppointmentServices.Add(new AppointmentService { AppointmentId = id, ServiceId = sid });

            _appointmentRepository.Update(existing);
            return RedirectToAction(nameof(Index));
        }

        // ── SOFT DELETE ──────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("appointments/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // Sets DeletedAt = DateTime.UtcNow — does NOT call Remove().
            // The global EF query filter on Appointment excludes it from all future queries.
            _appointmentRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        // ── AJAX SEARCH ENDPOINTS ─────────────────────────────────────────

        /// <summary>
        /// Returns active patients matching <paramref name="term"/> as JSON [{id, text}].
        /// Note: Patient does not carry DeletedAt yet; no soft-delete filter is applied here.
        /// The global EF query filter on Appointment already excludes soft-deleted appointments.
        /// </summary>
        [HttpGet]
        [Route("appointments/search-patients")]
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

        /// <summary>
        /// Returns active vets matching <paramref name="term"/> as JSON [{id, text}].
        /// Note: Vet does not carry DeletedAt yet; no soft-delete filter is applied here.
        /// </summary>
        [HttpGet]
        [Route("appointments/search-vets")]
        public IActionResult SearchVets(string? term)
        {
            var results = _vetRepository.GetAll()
                .Where(v => string.IsNullOrWhiteSpace(term)
                       || (v.FirstName ?? "").Contains(term, StringComparison.OrdinalIgnoreCase)
                       || (v.LastName  ?? "").Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.LastName)
                .Take(10)
                .Select(v => new { id = v.Id, text = $"Dr. {v.FirstName} {v.LastName}" })
                .ToList();

            return Json(results);
        }

        /// <summary>
        /// Returns active services matching <paramref name="term"/> as JSON [{id, text}].
        /// </summary>
        [HttpGet]
        [Route("appointments/search-services")]
        public IActionResult SearchServices(string? term)
        {
            var results = _serviceRepository.GetAll()
                .Where(s => string.IsNullOrWhiteSpace(term)
                       || (s.Name ?? "").Contains(term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.Name)
                .Take(15)
                .Select(s => new { id = s.Id, text = s.Name ?? "" })
                .ToList();

            return Json(results);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Resolves and stores display text for the autocomplete inputs so they
        /// can pre-fill correctly when a form is returned on validation failure.
        /// </summary>
        private void PopulateAutocompleteState(AppointmentFormModel model)
        {
            if (model.PatientId > 0)
            {
                var p = _patientRepository.GetById(model.PatientId);
                ViewBag.InitialPatientText = p?.Name ?? "";
            }
            if (model.VetId > 0)
            {
                var v = _vetRepository.GetById(model.VetId);
                ViewBag.InitialVetText = v != null ? $"Dr. {v.FirstName} {v.LastName}" : "";
            }
            // Re-hydrate pre-selected service tokens so Select2 can restore them on re-render.
            var selSvcs = _serviceRepository.GetAll()
                .Where(s => model.ServiceIds.Contains(s.Id))
                .Select(s => new { id = s.Id, text = s.Name ?? "" })
                .ToList();
            ViewBag.SelectedServicesJson = JsonSerializer.Serialize(selSvcs);
        }

        private static DateTime ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            DateTime.TryParseExact(
                value,
                new[] { "dd.MM.yyyy HH:mm", "MM/dd/yyyy hh:mm tt", "MM/dd/yyyy HH:mm" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result);
            return result;
        }
    }
}

