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
    [Route("api/services")]
    public class ServiceApiController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceApiController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<ServiceDTO>> GetAll([FromQuery] string? search = null)
        {
            var services = string.IsNullOrWhiteSpace(search)
                ? _serviceRepository.GetAll()
                : _serviceRepository.Search(search.Trim());

            var serviceDtos = services.Select(service => service.ToDTO())
                .Where(serviceDto => serviceDto != null)
                .Cast<ServiceDTO>()
                .ToList();

            return Ok(serviceDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ServiceDTO> GetById(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult<ServiceDTO> Create([FromBody] ServiceDTO model)
        {
            if (!IsValidServiceDto(model))
            {
                return ValidationProblem(ModelState);
            }

            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                EstimatedDurationMinutes = model.EstimatedDurationMinutes
            };

            _serviceRepository.Add(service);

            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int id, [FromBody] ServiceDTO model)
        {
            if (!IsValidServiceDto(model))
            {
                return ValidationProblem(ModelState);
            }

            var service = _serviceRepository.GetById(id);
            if (service == null)
            {
                return NotFound();
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.EstimatedDurationMinutes = model.EstimatedDurationMinutes;

            _serviceRepository.Update(service);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
            {
                return NotFound();
            }

            _serviceRepository.SoftDelete(id);
            return NoContent();
        }

        private bool IsValidServiceDto(ServiceDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
            }

            if (model.Price <= 0)
            {
                ModelState.AddModelError(nameof(model.Price), "Price must be greater than zero.");
            }

            if (model.EstimatedDurationMinutes <= 0)
            {
                ModelState.AddModelError(nameof(model.EstimatedDurationMinutes), "EstimatedDurationMinutes must be greater than zero.");
            }

            return ModelState.IsValid;
        }
    }
}
