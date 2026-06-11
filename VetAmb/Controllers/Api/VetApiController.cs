using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VetAmb.DTOs;
using VetAmb.Models;
using VetAmb.Repositories;

#nullable enable

namespace VetAmb.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/vets")]
    public class VetApiController : ControllerBase
    {
        private readonly IVetRepository _vetRepository;

        public VetApiController(IVetRepository vetRepository)
        {
            _vetRepository = vetRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<VetDTO>> GetAll([FromQuery] string? search = null)
        {
            var vets = string.IsNullOrWhiteSpace(search)
                ? _vetRepository.GetAll()
                : _vetRepository.Search(search.Trim());

            var vetDtos = vets.Select(vet => vet.ToDTO())
                .Where(vetDto => vetDto != null)
                .Cast<VetDTO>()
                .ToList();

            return Ok(vetDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<VetDTO> GetById(int id)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
            {
                return NotFound();
            }

            return Ok(vet.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<VetDTO> Create([FromBody] VetDTO model)
        {
            if (!TryParseSpecialization(model.Specialization, out var parsedSpecialization))
            {
                ModelState.AddModelError(nameof(model.Specialization), "Specialization is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            var vet = new Vet
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Specialization = parsedSpecialization,
                LicenseNumber = model.LicenseNumber,
                YearsOfExperience = model.YearsOfExperience,
                Phone = model.Phone,
                HourlyRate = model.HourlyRate,
                ClinicId = model.ClinicId
            };

            _vetRepository.Add(vet);

            return CreatedAtAction(nameof(GetById), new { id = vet.Id }, vet.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int id, [FromBody] VetDTO model)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
            {
                return NotFound();
            }

            if (!TryParseSpecialization(model.Specialization, out var parsedSpecialization))
            {
                ModelState.AddModelError(nameof(model.Specialization), "Specialization is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            vet.FirstName = model.FirstName;
            vet.LastName = model.LastName;
            vet.Specialization = parsedSpecialization;
            vet.LicenseNumber = model.LicenseNumber;
            vet.YearsOfExperience = model.YearsOfExperience;
            vet.Phone = model.Phone;
            vet.HourlyRate = model.HourlyRate;
            vet.ClinicId = model.ClinicId;

            _vetRepository.Update(vet);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            var vet = _vetRepository.GetById(id);
            if (vet == null)
            {
                return NotFound();
            }

            _vetRepository.SoftDelete(id);
            return NoContent();
        }

        private static bool TryParseSpecialization(string? value, out VeterinarySpecialization specialization)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                specialization = default;
                return false;
            }

            if (Enum.TryParse<VeterinarySpecialization>(value.Trim(), true, out specialization))
            {
                return true;
            }

            foreach (var enumValue in Enum.GetValues<VeterinarySpecialization>())
            {
                if (string.Equals(enumValue.GetDisplayName(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    specialization = enumValue;
                    return true;
                }
            }

            specialization = default;
            return false;
        }
    }
}
