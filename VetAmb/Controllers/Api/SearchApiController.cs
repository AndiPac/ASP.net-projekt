using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            var normalizedLower = normalizedTerm.ToLowerInvariant();
            var searchPages = GetSearchPages()
                .Where(page => page.Matches(normalizedTerm) && page.IsVisibleTo(User))
                .Take(8)
                .Select(page => new SearchResultItem
                {
                    Id = 0,
                    Text = page.Text,
                    Type = page.Type,
                    Url = page.Url,
                    Score = page.GetScore(normalizedLower)
                })
                .ToList();

            var clinics = await _context.Clinics
                .AsNoTracking()
                .Where(c =>
                    (c.Name != null && EF.Functions.Like(c.Name, $"%{normalizedTerm}%")) ||
                    (c.Address != null && EF.Functions.Like(c.Address, $"%{normalizedTerm}%")) ||
                    (c.Email != null && EF.Functions.Like(c.Email, $"%{normalizedTerm}%")) ||
                    (c.Phone != null && EF.Functions.Like(c.Phone, $"%{normalizedTerm}%")))
                .OrderBy(c => c.Name)
                .Take(5)
                .Select(c => new SearchResultItem
                {
                    Id = c.Id,
                    Text = c.Name!,
                    Type = "Klinika",
                    Url = $"/clinics/{c.Id}",
                    Score = GetEntityScore(normalizedLower, c.Name, c.Address, c.Email, c.Phone)
                })
                .ToListAsync();

            var vets = await _context.Vets
                .AsNoTracking()
                .Where(v =>
                    (v.FirstName != null && EF.Functions.Like(v.FirstName, $"%{normalizedTerm}%")) ||
                    (v.LastName != null && EF.Functions.Like(v.LastName, $"%{normalizedTerm}%")) ||
                    (v.LicenseNumber != null && EF.Functions.Like(v.LicenseNumber, $"%{normalizedTerm}%")) ||
                    (v.Phone != null && EF.Functions.Like(v.Phone, $"%{normalizedTerm}%")))
                .OrderBy(v => v.LastName)
                .ThenBy(v => v.FirstName)
                .Take(5)
                .Select(v => new SearchResultItem
                {
                    Id = v.Id,
                    Text = $"Dr. {v.FirstName} {v.LastName}",
                    Type = "Veterinar",
                    Url = $"/vets/{v.Id}",
                    Score = GetEntityScore(normalizedLower, v.FirstName, v.LastName, v.LicenseNumber, v.Phone)
                })
                .ToListAsync();

            var owners = await _context.Owners
                .AsNoTracking()
                .Where(o =>
                    (o.FirstName != null && EF.Functions.Like(o.FirstName, $"%{normalizedTerm}%")) ||
                    (o.LastName != null && EF.Functions.Like(o.LastName, $"%{normalizedTerm}%")) ||
                    (o.Email != null && EF.Functions.Like(o.Email, $"%{normalizedTerm}%")) ||
                    (o.Phone != null && EF.Functions.Like(o.Phone, $"%{normalizedTerm}%")) ||
                    (o.IdNumber != null && EF.Functions.Like(o.IdNumber, $"%{normalizedTerm}%")))
                .OrderBy(o => o.LastName)
                .ThenBy(o => o.FirstName)
                .Take(5)
                .Select(o => new SearchResultItem
                {
                    Id = o.Id,
                    Text = $"{o.FirstName} {o.LastName}",
                    Type = "Vlasnik",
                    Url = $"/owners/{o.Id}",
                    Score = GetEntityScore(normalizedLower, o.FirstName, o.LastName, o.Email, o.Phone, o.IdNumber)
                })
                .ToListAsync();

            var patients = await _context.Patients
                .AsNoTracking()
                .Where(p =>
                    (p.Name != null && EF.Functions.Like(p.Name, $"%{normalizedTerm}%")) ||
                    (p.Breed != null && EF.Functions.Like(p.Breed, $"%{normalizedTerm}%")) ||
                    (p.MicrochipId != null && EF.Functions.Like(p.MicrochipId, $"%{normalizedTerm}%")) ||
                    (p.Color != null && EF.Functions.Like(p.Color, $"%{normalizedTerm}%")))
                .OrderBy(p => p.Name)
                .Take(5)
                .Select(p => new SearchResultItem
                {
                    Id = p.Id,
                    Text = p.Name!,
                    Type = "Pacijent",
                    Url = $"/patients/{p.Id}",
                    Score = GetEntityScore(normalizedLower, p.Name, p.Breed, p.MicrochipId, p.Color)
                })
                .ToListAsync();

            var appointments = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Vet)
                .Where(a =>
                    (a.Reason != null && EF.Functions.Like(a.Reason, $"%{normalizedTerm}%")) ||
                    (a.Notes != null && EF.Functions.Like(a.Notes, $"%{normalizedTerm}%")) ||
                    (a.Patient != null && a.Patient.Name != null && EF.Functions.Like(a.Patient.Name, $"%{normalizedTerm}%")) ||
                    (a.Vet != null && a.Vet.FirstName != null && EF.Functions.Like(a.Vet.FirstName, $"%{normalizedTerm}%")) ||
                    (a.Vet != null && a.Vet.LastName != null && EF.Functions.Like(a.Vet.LastName, $"%{normalizedTerm}%")))
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(5)
                .Select(a => new SearchResultItem
                {
                    Id = a.Id,
                    Text = $"{(string.IsNullOrWhiteSpace(a.Reason) ? "Termin" : a.Reason)} • {a.AppointmentDateTime:dd.MM.yyyy HH:mm}",
                    Type = "Termin",
                    Url = $"/appointments/{a.Id}",
                    Score = GetEntityScore(normalizedLower, a.Reason, a.Patient != null ? a.Patient.Name : null, a.Vet != null ? a.Vet.FirstName : null, a.Vet != null ? a.Vet.LastName : null)
                })
                .ToListAsync();

            var services = await _context.Services
                .AsNoTracking()
                .Where(s =>
                    (s.Name != null && EF.Functions.Like(s.Name, $"%{normalizedTerm}%")) ||
                    (s.Description != null && EF.Functions.Like(s.Description, $"%{normalizedTerm}%")))
                .OrderBy(s => s.Name)
                .Take(5)
                .Select(s => new SearchResultItem
                {
                    Id = s.Id,
                    Text = s.Name!,
                    Type = "Usluga",
                    Url = $"/services/{s.Id}",
                    Score = GetEntityScore(normalizedLower, s.Name, s.Description)
                })
                .ToListAsync();

            var medicalRecords = await _context.MedicalRecords
                .AsNoTracking()
                .Include(r => r.Patient)
                .Where(r =>
                    (r.Diagnosis != null && EF.Functions.Like(r.Diagnosis, $"%{normalizedTerm}%")) ||
                    (r.Treatment != null && EF.Functions.Like(r.Treatment, $"%{normalizedTerm}%")) ||
                    (r.Notes != null && EF.Functions.Like(r.Notes, $"%{normalizedTerm}%")) ||
                    (r.Patient != null && r.Patient.Name != null && EF.Functions.Like(r.Patient.Name, $"%{normalizedTerm}%")))
                .OrderByDescending(r => r.RecordDate)
                .Take(5)
                .Select(r => new SearchResultItem
                {
                    Id = r.Id,
                    Text = $"{(string.IsNullOrWhiteSpace(r.Diagnosis) ? "Medicinski zapis" : r.Diagnosis)} • {r.RecordDate:dd.MM.yyyy}",
                    Type = "Karton",
                    Url = $"/medical-records/{r.Id}",
                    Score = GetEntityScore(normalizedLower, r.Diagnosis, r.Treatment, r.Patient != null ? r.Patient.Name : null)
                })
                .ToListAsync();

            var results = searchPages
                .Concat(clinics)
                .Concat(vets)
                .Concat(owners)
                .Concat(patients)
                .Concat(appointments)
                .Concat(services)
                .Concat(medicalRecords)
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Type)
                .ThenBy(item => item.Text)
                .ToList();

            return Ok(results);
        }

        private static IReadOnlyList<SearchPageItem> GetSearchPages()
        {
            return new[]
            {
                new SearchPageItem("Nadzorna ploča", "Izbornik", "/", "dashboard,pocetna,pocetna stranica,nadzorna ploca,home"),
                new SearchPageItem("Termini", "Izbornik", "/appointments", "termini,appointments,raspored,pregledi"),
                new SearchPageItem("Klinike", "Izbornik", "/clinics", "klinike,clinics,ambulante"),
                new SearchPageItem("Zdravstveni kartoni", "Izbornik", "/medical-records", "zdravstveni kartoni,medicinski zapisi,medical records,kartoni"),
                new SearchPageItem("Vlasnici", "Izbornik", "/owners", "vlasnici,owners,klijenti"),
                new SearchPageItem("Pacijenti", "Izbornik", "/patients", "pacijenti,patients,zivotinje,ljubimci"),
                new SearchPageItem("Usluge", "Izbornik", "/services", "usluge,services,tretmani"),
                new SearchPageItem("Veterinari", "Izbornik", "/vets", "veterinari,vets,doktori,dr"),
                new SearchPageItem("Novi termin", "Stranica", "/appointments/create", "novi termin,dodaj termin,zakazi termin", "Administrator", "Vet"),
                new SearchPageItem("Nova klinika", "Stranica", "/clinics/create", "nova klinika,dodaj kliniku", "Administrator"),
                new SearchPageItem("Novi karton", "Stranica", "/medical-records/create", "novi karton,novi zapis,dodaj karton,medicinski zapis", "Administrator", "Vet"),
                new SearchPageItem("Novi vlasnik", "Stranica", "/owners/create", "novi vlasnik,dodaj vlasnika", "Administrator", "Vet"),
                new SearchPageItem("Novi pacijent", "Stranica", "/patients/create", "novi pacijent,dodaj pacijenta", "Administrator", "Vet"),
                new SearchPageItem("Nova usluga", "Stranica", "/services/create", "nova usluga,dodaj uslugu", "Administrator"),
                new SearchPageItem("Novi veterinar", "Stranica", "/vets/create", "novi veterinar,dodaj veterinara", "Administrator")
            };
        }

        private sealed class SearchResultItem
        {
            public int Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public int Score { get; set; }
        }

        private sealed class SearchPageItem
        {
            public SearchPageItem(string text, string type, string url, string keywords, params string[] requiredRoles)
            {
                Text = text;
                Type = type;
                Url = url;
                Keywords = keywords;
                RequiredRoles = requiredRoles ?? Array.Empty<string>();
            }

            public string Text { get; }
            public string Type { get; }
            public string Url { get; }
            public string Keywords { get; }
            public IReadOnlyList<string> RequiredRoles { get; }

            public bool Matches(string term)
            {
                return Text.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || Keywords.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || Url.Contains(term, StringComparison.OrdinalIgnoreCase);
            }

            public int GetScore(string normalizedTerm)
            {
                var score = 0;

                if (Text.Equals(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 100;
                }

                if (Text.StartsWith(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 50;
                }

                if (Keywords.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 20;
                }

                if (Url.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 10;
                }

                score += Type switch
                {
                    "Izbornik" => 30,
                    "Stranica" => 20,
                    "Klinike" => 0,
                    _ => 0
                };

                return score;
            }

            public bool IsVisibleTo(ClaimsPrincipal user)
            {
                if (RequiredRoles.Count == 0)
                {
                    return true;
                }

                if (user?.Identity?.IsAuthenticated != true)
                {
                    return false;
                }

                return RequiredRoles.Any(user.IsInRole);
            }
        }

        private static int GetEntityScore(string normalizedTerm, params string?[] values)
        {
            var score = 0;

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (value.Equals(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 100;
                }

                if (value.StartsWith(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 40;
                }

                if (value.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase))
                {
                    score += 15;
                }
            }

            return score;
        }
    }
}