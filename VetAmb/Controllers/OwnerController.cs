using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VetAmb.Models;
using VetAmb.Repositories;
using VetAmb.ViewModels;

namespace VetAmb.Controllers
{
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository   _ownerRepository;
        private readonly IClinicRepository  _clinicRepository;

        public OwnerController(
            IOwnerRepository  ownerRepository,
            IClinicRepository clinicRepository)
        {
            _ownerRepository  = ownerRepository;
            _clinicRepository = clinicRepository;
        }

        // ── LIST ─────────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("owners")]
        public IActionResult Index()
        {
            var owners = _ownerRepository.GetAll();
            return View(owners);
        }

        // ── DETAILS ──────────────────────────────────────────────────────

        [AllowAnonymous]
        [Route("owners/{id:int}")]
        public IActionResult Details(int id)
        {
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
                return NotFound();
            return View(owner);
        }

        // ── CREATE ───────────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("owners/create")]
        public IActionResult Create()
        {
            return View(new OwnerFormModel());
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("owners/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(OwnerFormModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            var owner = new Owner
            {
                FirstName        = model.FirstName,
                LastName         = model.LastName,
                Email            = model.Email,
                Phone            = model.Phone,
                Address          = model.Address,
                IdNumber         = model.IdNumber,
                ClinicId         = model.ClinicId,
                RegistrationDate = DateTime.UtcNow
            };

            _ownerRepository.Add(owner);
            return RedirectToAction(nameof(Details), new { id = owner.Id });
        }

        // ── EDIT ─────────────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpGet]
        [Route("owners/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
                return NotFound();

            var model = new OwnerFormModel
            {
                Id        = owner.Id,
                FirstName = owner.FirstName,
                LastName  = owner.LastName,
                Email     = owner.Email,
                Phone     = owner.Phone,
                Address   = owner.Address,
                IdNumber  = owner.IdNumber,
                ClinicId  = owner.ClinicId
            };

            // Pre-populate autocomplete display text for Clinic field.
            ViewBag.InitialClinicText = owner.Clinic?.Name ?? "";

            return View(model);
        }

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("owners/{id:int}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, OwnerFormModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                PopulateAutocompleteState(model);
                return View(model);
            }

            // Fetch the tracked entity — never touch Id, RegistrationDate, or DeletedAt.
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
                return NotFound();

            owner.FirstName = model.FirstName;
            owner.LastName  = model.LastName;
            owner.Email     = model.Email;
            owner.Phone     = model.Phone;
            owner.Address   = model.Address;
            owner.IdNumber  = model.IdNumber;
            owner.ClinicId  = model.ClinicId;

            _ownerRepository.Update(owner);
            return RedirectToAction(nameof(Details), new { id = owner.Id });
        }

        // ── SOFT DELETE ───────────────────────────────────────────────────

        [Authorize(Roles = "Administrator,Vet")]
        [HttpPost]
        [Route("owners/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _ownerRepository.SoftDelete(id);
            return RedirectToAction(nameof(Index));
        }

        // ── AJAX SEARCH ENDPOINTS ─────────────────────────────────────────

        /// <summary>
        /// Returns clinics matching <paramref name="term"/> as JSON [{id, text}].
        /// Used by the ClinicId autocomplete field in Owner Create/Edit forms.
        /// </summary>
        [HttpGet]
        [Route("owners/search-clinics")]
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
        private void PopulateAutocompleteState(OwnerFormModel model)
        {
            if (model.ClinicId > 0)
            {
                var c = _clinicRepository.GetById(model.ClinicId);
                ViewBag.InitialClinicText = c?.Name ?? "";
            }
        }
    }
}

