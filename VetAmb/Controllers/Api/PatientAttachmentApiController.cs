using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetAmb.Data;
using VetAmb.Models;

#nullable enable

namespace VetAmb.Controllers.Api
{
    [ApiController]
    [Route("api/patients/{patientId:int}/attachments")]
    [Authorize]
    public class PatientAttachmentApiController : ControllerBase
    {
        private readonly VetAmbDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PatientAttachmentApiController(VetAmbDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator,Vet,User")]
        public async Task<IActionResult> GetAll(int patientId)
        {
            var attachments = await _context.PatientAttachments
                .AsNoTracking()
                .Where(attachment => attachment.PatientId == patientId)
                .Select(attachment => new
                {
                    attachment.Id,
                    attachment.FileName,
                    attachment.FilePath,
                    attachment.ContentType,
                    attachment.UploadedAt
                })
                .ToListAsync();

            return Ok(attachments);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Vet")]
        public async Task<IActionResult> Upload(int patientId, [FromForm] IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required.");
            }

            var patientExists = await _context.Patients.AnyAsync(patient => patient.Id == patientId);
            if (!patientExists)
            {
                return NotFound();
            }

            var uploadDirectory = Path.Combine(
                _env.WebRootPath,
                "uploads",
                "patients",
                patientId.ToString());

            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullFilePath = Path.Combine(uploadDirectory, uniqueFileName);

            await using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeFilePath = $"/uploads/patients/{patientId}/{uniqueFileName}";

            var attachment = new PatientAttachment
            {
                FileName = file.FileName,
                FilePath = relativeFilePath,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow,
                PatientId = patientId
            };

            _context.PatientAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully", id = attachment.Id });
        }

        [HttpDelete("{attachmentId:int}")]
        [Authorize(Roles = "Administrator,Vet")]
        public async Task<IActionResult> Delete(int patientId, int attachmentId)
        {
            var attachment = await _context.PatientAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.PatientId == patientId);

            if (attachment == null)
            {
                return NotFound();
            }

            var relativePath = attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(_env.WebRootPath, relativePath);

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.PatientAttachments.Remove(attachment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
