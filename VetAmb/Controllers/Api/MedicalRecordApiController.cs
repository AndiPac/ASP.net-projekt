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
    [Route("api/medical-records")]
    public class MedicalRecordApiController : ControllerBase
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordApiController(IMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<MedicalRecordDTO>> GetAll([FromQuery] string? search = null)
        {
            var records = string.IsNullOrWhiteSpace(search)
                ? _medicalRecordRepository.GetAll()
                : _medicalRecordRepository.Search(search.Trim());

            var recordDtos = records.Select(record => record.ToDTO())
                .Where(recordDto => recordDto != null)
                .Cast<MedicalRecordDTO>()
                .ToList();

            return Ok(recordDtos);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<MedicalRecordDTO> GetById(int id)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
            {
                return NotFound();
            }

            return Ok(record.ToDTO());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Vet")]
        public ActionResult<MedicalRecordDTO> Create([FromBody] MedicalRecordDTO model)
        {
            if (model.RecordDate == default)
            {
                ModelState.AddModelError(nameof(model.RecordDate), "RecordDate is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            var record = new MedicalRecord
            {
                Diagnosis = model.Diagnosis,
                Treatment = model.Treatment,
                RecordDate = model.RecordDate,
                Notes = model.Notes,
                PatientId = model.PatientId
            };

            _medicalRecordRepository.Add(record);

            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record.ToDTO());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Update(int id, [FromBody] MedicalRecordDTO model)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
            {
                return NotFound();
            }

            if (model.RecordDate == default)
            {
                ModelState.AddModelError(nameof(model.RecordDate), "RecordDate is required and must be a valid date.");
                return ValidationProblem(ModelState);
            }

            record.Diagnosis = model.Diagnosis;
            record.Treatment = model.Treatment;
            record.RecordDate = model.RecordDate;
            record.Notes = model.Notes;
            record.PatientId = model.PatientId;

            _medicalRecordRepository.Update(record);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public IActionResult Delete(int id)
        {
            var record = _medicalRecordRepository.GetById(id);
            if (record == null)
            {
                return NotFound();
            }

            _medicalRecordRepository.SoftDelete(id);
            return NoContent();
        }
    }
}
