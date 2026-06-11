using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetAmb.Data;
using VetAmb.DTOs;
using VetAmb.Models;
using VetAmb.Repositories;

#nullable enable

namespace VetAmb.Controllers.Api
{
    [ApiController]
    [Authorize]
    [Route("api/appointments")]
    public class AppointmentApiController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly VetAmbDbContext _context;

        public AppointmentApiController(
            IAppointmentRepository appointmentRepository,
            VetAmbDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<AppointmentDTO>> GetAll([FromQuery] string? search = null)
        {
            var appointments = string.IsNullOrWhiteSpace(search)
                ? _appointmentRepository.GetAll()
                : _appointmentRepository.Search(search.Trim());

            var appointmentDtos = appointments.Select(appointment => appointment.ToDTO())
                .Where(appointmentDto => appointmentDto != null)
                .Cast<AppointmentDTO>()
                .ToList();

            return Ok(appointmentDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<AppointmentDTO> GetById(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Vet")]
        public ActionResult<AppointmentDTO> Create([FromBody] AppointmentDTO model)
        {
            if (model.AppointmentDateTime == default)
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "AppointmentDateTime is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            if (!TryParseStatus(model.Status, out var parsedStatus))
            {
                ModelState.AddModelError(nameof(model.Status), "Status is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            var appointment = new Appointment
            {
                AppointmentDateTime = model.AppointmentDateTime,
                Reason = model.Reason,
                Status = parsedStatus,
                Notes = model.Notes,
                RescheduleReason = model.RescheduleReason,
                PatientId = model.PatientId,
                VetId = model.VetId
            };

            _appointmentRepository.Add(appointment);

            foreach (var serviceId in model.ServiceIds)
            {
                _context.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.Id,
                    ServiceId = serviceId
                });
            }

            if (model.ServiceIds.Any())
            {
                _context.SaveChanges();
            }

            var createdAppointment = _appointmentRepository.GetById(appointment.Id);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, createdAppointment?.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Update(int id, [FromBody] AppointmentDTO model)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (model.AppointmentDateTime == default)
            {
                ModelState.AddModelError(nameof(model.AppointmentDateTime), "AppointmentDateTime is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            if (!TryParseStatus(model.Status, out var parsedStatus))
            {
                ModelState.AddModelError(nameof(model.Status), "Status is required and must be a valid enum value.");
                return ValidationProblem(ModelState);
            }

            appointment.AppointmentDateTime = model.AppointmentDateTime;
            appointment.Reason = model.Reason;
            appointment.Status = parsedStatus;
            appointment.Notes = model.Notes;
            appointment.RescheduleReason = model.RescheduleReason;
            appointment.PatientId = model.PatientId;
            appointment.VetId = model.VetId;

            var oldServices = _context.AppointmentServices
                .Where(appointmentService => appointmentService.AppointmentId == id)
                .ToList();

            _context.AppointmentServices.RemoveRange(oldServices);

            foreach (var serviceId in model.ServiceIds)
            {
                _context.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = id,
                    ServiceId = serviceId
                });
            }

            _appointmentRepository.Update(appointment);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Delete(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _appointmentRepository.SoftDelete(id);
            return NoContent();
        }

        private static bool TryParseStatus(string? value, out AppointmentStatus status)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                status = default;
                return false;
            }

            if (Enum.TryParse<AppointmentStatus>(value.Trim(), true, out status))
            {
                return true;
            }

            foreach (var enumValue in Enum.GetValues<AppointmentStatus>())
            {
                if (string.Equals(enumValue.GetDisplayName(), value.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    status = enumValue;
                    return true;
                }
            }

            status = default;
            return false;
        }
    }
}
