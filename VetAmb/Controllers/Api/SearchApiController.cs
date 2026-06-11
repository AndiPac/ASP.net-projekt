using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetAmb.Data;

namespace VetAmb.Controllers.Api
{
    [ApiController]
    [Route("api/search")]
    public class SearchApiController : ControllerBase
    {
        private readonly VetAmbDbContext _context;

        public SearchApiController(VetAmbDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
            {
                return Ok(Array.Empty<SearchResultItem>());
            }

            var normalizedTerm = term.Trim();

            var clinics = await _context.Clinics
                .AsNoTracking()
                .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{normalizedTerm}%"))
                .OrderBy(c => c.Name)
                .Take(5)
                .Select(c => new SearchResultItem
                {
                    Id = c.Id,
                    Text = c.Name!,
                    Type = "Klinika",
                    Url = $"/clinics/{c.Id}"
                })
                .ToListAsync();

            var vets = await _context.Vets
                .AsNoTracking()
                .Where(v =>
                    (v.FirstName != null && EF.Functions.Like(v.FirstName, $"%{normalizedTerm}%")) ||
                    (v.LastName != null && EF.Functions.Like(v.LastName, $"%{normalizedTerm}%")))
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .Take(5)
                .Select(v => new SearchResultItem
                {
                    Id = v.Id,
                    Text = $"Dr. {v.FirstName} {v.LastName}",
                    Type = "Veterinar",
                    Url = $"/vets/{v.Id}"
                })
                .ToListAsync();

            var owners = await _context.Owners
                .AsNoTracking()
                .Where(o =>
                    (o.FirstName != null && EF.Functions.Like(o.FirstName, $"%{normalizedTerm}%")) ||
                    (o.LastName != null && EF.Functions.Like(o.LastName, $"%{normalizedTerm}%")))
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .Take(5)
                .Select(o => new SearchResultItem
                {
                    Id = o.Id,
                    Text = $"{o.FirstName} {o.LastName}",
                    Type = "Vlasnik",
                    Url = $"/owners/{o.Id}"
                })
                .ToListAsync();

            var patients = await _context.Patients
                .AsNoTracking()
                .Where(p => p.Name != null && EF.Functions.Like(p.Name, $"%{normalizedTerm}%"))
                .OrderBy(p => p.Name)
                .Take(5)
                .Select(p => new SearchResultItem
                {
                    Id = p.Id,
                    Text = $"{p.Name} (Patient)",
                    Type = "Pacijent",
                    Url = $"/patients/{p.Id}"
                })
                .ToListAsync();

            var results = clinics
                .Concat(vets)
                .Concat(owners)
                .Concat(patients)
                .ToList();

            return Ok(results);
        }

        private sealed class SearchResultItem
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }
    }
}