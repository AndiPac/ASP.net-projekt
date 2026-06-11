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
    [Route("api/clinics")]
    public class ClinicApiController : ControllerBase
    {
        private readonly IClinicRepository _clinicRepository;

        public ClinicApiController(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<ClinicDTO>> GetAll([FromQuery] string? search = null)
        {
            var clinics = string.IsNullOrWhiteSpace(search)
                ? _clinicRepository.GetAll()
                : _clinicRepository.Search(search.Trim());

            var clinicDtos = clinics.Select(clinic => clinic.ToDTO())
                .Where(clinicDto => clinicDto != null)
                .Cast<ClinicDTO>()
                .ToList();

            return Ok(clinicDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ClinicDTO> GetById(int id)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
            {
                return NotFound();
            }

            return Ok(clinic.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<ClinicDTO> Create([FromBody] ClinicDTO model)
        {
            if (model.FoundationDate == default)
            {
                ModelState.AddModelError(nameof(model.FoundationDate), "FoundationDate is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            var clinic = new Clinic
            {
                Name = model.Name,
                Address = model.Address,
                Phone = model.Phone,
                Email = model.Email,
                RegistrationNumber = model.RegistrationNumber,
                FoundationDate = model.FoundationDate,
                MaxCapacity = model.MaxCapacity
            };

            _clinicRepository.Add(clinic);

            return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, clinic.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int id, [FromBody] ClinicDTO model)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
            {
                return NotFound();
            }

            if (model.FoundationDate == default)
            {
                ModelState.AddModelError(nameof(model.FoundationDate), "FoundationDate is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            clinic.Name = model.Name;
            clinic.Address = model.Address;
            clinic.Phone = model.Phone;
            clinic.Email = model.Email;
            clinic.RegistrationNumber = model.RegistrationNumber;
            clinic.MaxCapacity = model.MaxCapacity;
            clinic.FoundationDate = model.FoundationDate;

            _clinicRepository.Update(clinic);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            var clinic = _clinicRepository.GetById(id);
            if (clinic == null)
            {
                return NotFound();
            }

            _clinicRepository.SoftDelete(id);
            return NoContent();
        }

    }
}
