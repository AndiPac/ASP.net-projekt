using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetAmb.DTOs;
using VetAmb.Models;
using VetAmb.Repositories;

#nullable enable

namespace VetAmb.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/patients")]
    public class PatientApiController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientApiController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<PatientDTO>> GetAll([FromQuery] string? search = null)
        {
            var patients = string.IsNullOrWhiteSpace(search)
                ? _patientRepository.GetAll()
                : _patientRepository.Search(search.Trim());

            var patientDtos = patients.Select(patient => patient.ToDTO())
                .Where(patientDto => patientDto != null)
                .Cast<PatientDTO>()
                .ToList();

            return Ok(patientDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<PatientDTO> GetById(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Vet")]
        public ActionResult<PatientDTO> Create([FromBody] PatientDTO model)
        {
            if (model.DateOfBirth == default)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "DateOfBirth is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            if (!TryParseSpecies(model.Species, out var parsedSpecies))
            {
                ModelState.AddModelError(nameof(model.Species), "Species is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            var patient = new Patient
            {
                Name = model.Name,
                Species = parsedSpecies,
                Breed = model.Breed,
                DateOfBirth = model.DateOfBirth,
                Weight = model.Weight,
                MicrochipId = model.MicrochipId,
                Color = model.Color,
                OwnerId = model.OwnerId
            };

            _patientRepository.Add(patient);

            return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Update(int id, [FromBody] PatientDTO model)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            if (model.DateOfBirth == default)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "DateOfBirth is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            if (!TryParseSpecies(model.Species, out var parsedSpecies))
            {
                ModelState.AddModelError(nameof(model.Species), "Species is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            patient.Name = model.Name;
            patient.Species = parsedSpecies;
            patient.Breed = model.Breed;
            patient.DateOfBirth = model.DateOfBirth;
            patient.Weight = model.Weight;
            patient.MicrochipId = model.MicrochipId;
            patient.Color = model.Color;
            patient.OwnerId = model.OwnerId;

            _patientRepository.Update(patient);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Delete(int id)
        {
            var patient = _patientRepository.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            _patientRepository.SoftDelete(id);
            return NoContent();
        }

        private static bool TryParseSpecies(string? value, out AnimalSpecies species)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                species = default;
                return false;
            }

            if (Enum.TryParse<AnimalSpecies>(value.Trim(), true, out species))
            {
                return true;
            }

            foreach (var enumValue in Enum.GetValues<AnimalSpecies>())
            {
                if (string.Equals(enumValue.GetDisplayName(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    species = enumValue;
                    return true;
                }
            }

            species = default;
            return false;
        }
    }
}
