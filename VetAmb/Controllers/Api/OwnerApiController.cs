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
    [Route("api/owners")]
    public class OwnerApiController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerApiController(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<OwnerDTO>> GetAll([FromQuery] string? search = null)
        {
            var owners = string.IsNullOrWhiteSpace(search)
                ? _ownerRepository.GetAll()
                : _ownerRepository.Search(search.Trim());

            var ownerDtos = owners.Select(owner => owner.ToDTO())
                .Where(ownerDto => ownerDto != null)
                .Cast<OwnerDTO>()
                .ToList();

            return Ok(ownerDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<OwnerDTO> GetById(int id)
        {
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
            {
                return NotFound();
            }

            return Ok(owner.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Vet")]
        public ActionResult<OwnerDTO> Create([FromBody] OwnerDTO model)
        {
            if (!IsValidOwnerDto(model))
            {
                return ValidationProblem(ModelState);
            }

            var owner = new Owner
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                IdNumber = model.IdNumber,
                ClinicId = model.ClinicId,
                RegistrationDate = DateTime.UtcNow
            };

            _ownerRepository.Add(owner);

            return CreatedAtAction(nameof(GetById), new { id = owner.Id }, owner.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Update(int id, [FromBody] OwnerDTO model)
        {
            if (!IsValidOwnerDto(model))
            {
                return ValidationProblem(ModelState);
            }

            var owner = _ownerRepository.GetById(id);
            if (owner == null)
            {
                return NotFound();
            }

            owner.FirstName = model.FirstName;
            owner.LastName = model.LastName;
            owner.Email = model.Email;
            owner.Phone = model.Phone;
            owner.Address = model.Address;
            owner.IdNumber = model.IdNumber;
            owner.ClinicId = model.ClinicId;

            _ownerRepository.Update(owner);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Delete(int id)
        {
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
            {
                return NotFound();
            }

            _ownerRepository.SoftDelete(id);
            return NoContent();
        }

        private bool IsValidOwnerDto(OwnerDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                ModelState.AddModelError(nameof(model.FirstName), "FirstName is required.");
            }

            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                ModelState.AddModelError(nameof(model.LastName), "LastName is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email is required and must be valid.");
            }

            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError(nameof(model.Phone), "Phone is required.");
            }

            if (string.IsNullOrWhiteSpace(model.Address))
            {
                ModelState.AddModelError(nameof(model.Address), "Address is required.");
            }

            if (string.IsNullOrWhiteSpace(model.IdNumber))
            {
                ModelState.AddModelError(nameof(model.IdNumber), "IdNumber is required.");
            }

            if (model.ClinicId <= 0)
            {
                ModelState.AddModelError(nameof(model.ClinicId), "ClinicId must be greater than zero.");
            }

            return ModelState.IsValid;
        }
    }
}
