using OpenAI.Chat;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VetAmb.Data;
using AppChatMessage = VetAmb.Models.ChatMessage;

namespace VetAmb.Services;

public class AiChatbotService
{
    private const string SystemPrompt = "You are a helpful, professional assistant for the VetAmb veterinary clinic. Answer questions about pets and our services.";

    private readonly ChatClient _chatClient;
    private readonly ILogger<AiChatbotService> _logger;
    private readonly VetAmbDbContext _dbContext;

    public AiChatbotService(IConfiguration configuration, VetAmbDbContext dbContext, ILogger<AiChatbotService> logger)
    {
        _logger = logger;
        _dbContext = dbContext;

        var apiKey = configuration["OpenAI:ApiKey"] ?? configuration["OPENAI_API_KEY"];
        var model = configuration["OpenAI:Model"];

        if (string.IsNullOrWhiteSpace(model))
        {
            model = "gpt-4o-mini";
        }

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("PLACEHOLDER", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Set OpenAI:ApiKey in configuration or OPENAI_API_KEY in environment variables.");
        }

        _chatClient = new ChatClient(model, apiKey);
    }

    public async Task<string> GetResponseAsync(List<AppChatMessage>? conversation, ClaimsPrincipal? user, CancellationToken cancellationToken = default)
    {
        var latestUserMessage = conversation?
            .Where(m => !string.IsNullOrWhiteSpace(m?.Content) &&
                        string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase))
            .Select(m => m!.Content.Trim())
            .LastOrDefault() ?? string.Empty;

        var publicContext = await BuildPublicContextAsync(latestUserMessage, user, cancellationToken);

        var sdkMessages = new List<OpenAI.Chat.ChatMessage>
        {
            new SystemChatMessage(SystemPrompt),
            new SystemChatMessage($"Use the following public VetAmb data context when relevant. If information is missing from context, say that clearly and suggest where user can check in the app.\n\n{publicContext}")
        };

        if (conversation is not null)
        {
            foreach (var message in conversation)
            {
                if (string.IsNullOrWhiteSpace(message?.Content))
                {
                    continue;
                }

                var role = (message.Role ?? string.Empty).Trim().ToLowerInvariant();
                var content = message.Content.Trim();

                if (role == "assistant")
                {
                    sdkMessages.Add(new AssistantChatMessage(content));
                    continue;
                }

                if (role == "system")
                {
                    // Ignore caller-provided system messages so the strict system prompt is always first.
                    continue;
                }

                sdkMessages.Add(new UserChatMessage(content));
            }
        }

        if (sdkMessages.Count == 1)
        {
            sdkMessages.Add(new UserChatMessage("Hello"));
        }

        var completion = await _chatClient.CompleteChatAsync(sdkMessages, cancellationToken: cancellationToken);

        var answer = string.Concat(completion.Value.Content.Select(c => c.Text));

        if (string.IsNullOrWhiteSpace(answer))
        {
            _logger.LogWarning("OpenAI completion returned an empty answer.");
            return "Trenutno nemam odgovor. Molim pokušajte ponovno.";
        }

        return answer.Trim();
    }

    private async Task<string> BuildPublicContextAsync(string userQuery, ClaimsPrincipal? user, CancellationToken cancellationToken)
    {
        try
        {
            _ = user;

            var normalizedTerms = (userQuery ?? string.Empty)
                .Split(new[] { ' ', ',', '.', ';', ':', '!', '?', '/', '\\', '-', '_', '"', '\'', '(', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLowerInvariant())
                .Where(t => t.Length >= 2)
                .Distinct()
                .Take(6)
                .ToList();

            bool Matches(string text)
            {
                if (normalizedTerms.Count == 0)
                {
                    return true;
                }

                var target = (text ?? string.Empty).ToLowerInvariant();
                return normalizedTerms.Any(term => target.Contains(term));
            }

            var clinics = await _dbContext.Clinics
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name, c.Address, c.Phone })
                .Take(200)
                .ToListAsync(cancellationToken);

            var vets = await _dbContext.Vets
                .AsNoTracking()
                .Select(v => new { v.Id, v.FirstName, v.LastName, v.Specialization, v.ClinicId })
                .Take(300)
                .ToListAsync(cancellationToken);

            var services = await _dbContext.Services
                .AsNoTracking()
                .Select(s => new { s.Id, s.Name, s.Description, s.Price, s.EstimatedDurationMinutes })
                .Take(300)
                .ToListAsync(cancellationToken);

            var owners = await _dbContext.Owners
                .AsNoTracking()
                .Select(o => new { o.Id, o.FirstName, o.LastName, o.Phone, o.Email, o.ClinicId })
                .Take(300)
                .ToListAsync(cancellationToken);

            var patients = await _dbContext.Patients
                .AsNoTracking()
                .Select(p => new { p.Id, p.Name, p.Species, p.Breed, p.Color, p.OwnerId })
                .Take(300)
                .ToListAsync(cancellationToken);

            var appointments = await _dbContext.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                .Include(a => a.Vet)
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(180)
                .Select(a => new
                {
                    a.Id,
                    a.AppointmentDateTime,
                    a.Reason,
                    Patient = a.Patient != null ? a.Patient.Name : string.Empty,
                    Vet = a.Vet != null ? (a.Vet.FirstName + " " + a.Vet.LastName) : string.Empty,
                    a.Status
                })
                .ToListAsync(cancellationToken);

            var records = await _dbContext.MedicalRecords
                .AsNoTracking()
                .Include(r => r.Patient)
                .OrderByDescending(r => r.RecordDate)
                .Take(180)
                .Select(r => new
                {
                    r.Id,
                    r.RecordDate,
                    r.Diagnosis,
                    r.Treatment,
                    Patient = r.Patient != null ? r.Patient.Name : string.Empty,
                    r.PatientId
                })
                .ToListAsync(cancellationToken);

            var vetCountsByClinic = await _dbContext.Vets
                .AsNoTracking()
                .GroupBy(v => v.ClinicId)
                .Select(g => new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var ownerCountsByClinic = await _dbContext.Owners
                .AsNoTracking()
                .GroupBy(o => o.ClinicId)
                .Select(g => new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var patientCountsByClinic = await (
                from p in _dbContext.Patients.AsNoTracking()
                join o in _dbContext.Owners.AsNoTracking() on p.OwnerId equals o.Id
                group p by o.ClinicId into g
                select new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var appointmentCountsByClinic = await (
                from a in _dbContext.Appointments.AsNoTracking()
                join v in _dbContext.Vets.AsNoTracking() on a.VetId equals v.Id
                group a by v.ClinicId into g
                select new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var recordCountsByClinic = await (
                from r in _dbContext.MedicalRecords.AsNoTracking()
                join p in _dbContext.Patients.AsNoTracking() on r.PatientId equals p.Id
                join o in _dbContext.Owners.AsNoTracking() on p.OwnerId equals o.Id
                group r by o.ClinicId into g
                select new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var serviceUsageByClinic = await (
                from aps in _dbContext.AppointmentServices.AsNoTracking()
                join a in _dbContext.Appointments.AsNoTracking() on aps.AppointmentId equals a.Id
                join v in _dbContext.Vets.AsNoTracking() on a.VetId equals v.Id
                group aps by v.ClinicId into g
                select new { ClinicId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ClinicId, x => x.Count, cancellationToken);

            var appointmentCountsByVet = await _dbContext.Appointments
                .AsNoTracking()
                .GroupBy(a => a.VetId)
                .Select(g => new { VetId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.VetId, x => x.Count, cancellationToken);

            var serviceUsageByService = await _dbContext.AppointmentServices
                .AsNoTracking()
                .GroupBy(aps => aps.ServiceId)
                .Select(g => new { ServiceId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ServiceId, x => x.Count, cancellationToken);

            var patientCountsByOwner = await _dbContext.Patients
                .AsNoTracking()
                .GroupBy(p => p.OwnerId)
                .Select(g => new { OwnerId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.OwnerId, x => x.Count, cancellationToken);

            var appointmentCountsByPatient = await _dbContext.Appointments
                .AsNoTracking()
                .GroupBy(a => a.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PatientId, x => x.Count, cancellationToken);

            var recordCountsByPatient = await _dbContext.MedicalRecords
                .AsNoTracking()
                .GroupBy(r => r.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PatientId, x => x.Count, cancellationToken);

            var attachmentCountsByPatient = await _dbContext.PatientAttachments
                .AsNoTracking()
                .GroupBy(a => a.PatientId)
                .Select(g => new { PatientId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PatientId, x => x.Count, cancellationToken);

            var appointmentServiceCountsByAppointment = await _dbContext.AppointmentServices
                .AsNoTracking()
                .GroupBy(aps => aps.AppointmentId)
                .Select(g => new { AppointmentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.AppointmentId, x => x.Count, cancellationToken);

            static int CountOrZero(Dictionary<int, int> source, int key)
            {
                return source.TryGetValue(key, out var value) ? value : 0;
            }

            var clinicById = clinics.ToDictionary(c => c.Id, c => c.Name ?? string.Empty);
            var ownerById = owners.ToDictionary(o => o.Id, o => $"{o.FirstName} {o.LastName}");

            var clinicLines = clinics
                .Where(c => Matches($"{c.Name} {c.Address} {c.Phone}"))
                .Take(12)
                .Select(c =>
                {
                    var vetCount = CountOrZero(vetCountsByClinic, c.Id);
                    var ownerCount = CountOrZero(ownerCountsByClinic, c.Id);
                    var patientCount = CountOrZero(patientCountsByClinic, c.Id);
                    var appointmentCount = CountOrZero(appointmentCountsByClinic, c.Id);
                    var recordCount = CountOrZero(recordCountsByClinic, c.Id);
                    var serviceUsageCount = CountOrZero(serviceUsageByClinic, c.Id);
                    return $"- {c.Name} | adresa: {c.Address} | telefon: {c.Phone} | veterinari: {vetCount} | vlasnici: {ownerCount} | pacijenti: {patientCount} | termini: {appointmentCount} | kartoni: {recordCount} | korištenja usluga: {serviceUsageCount}";
                })
                .ToList();

            var vetLines = vets
                .Where(v => Matches($"{v.FirstName} {v.LastName} {v.Specialization}"))
                .Take(16)
                .Select(v =>
                {
                    var clinicName = clinicById.TryGetValue(v.ClinicId, out var clinic) ? clinic : "Nepoznata klinika";
                    var appointmentCount = CountOrZero(appointmentCountsByVet, v.Id);
                    return $"- Dr. {v.FirstName} {v.LastName} | specijalizacija: {v.Specialization} | klinika: {clinicName} | termini: {appointmentCount}";
                })
                .ToList();

            var serviceLines = services
                .Where(s => Matches($"{s.Name} {s.Description} {s.Price}"))
                .Take(16)
                .Select(s =>
                {
                    var usageCount = CountOrZero(serviceUsageByService, s.Id);
                    return $"- {s.Name} | cijena: {s.Price:0.##} EUR | trajanje: {s.EstimatedDurationMinutes} min | korištenja: {usageCount} | opis: {s.Description}";
                })
                .ToList();

            var ownerLines = owners
                .Where(o => Matches($"{o.FirstName} {o.LastName} {o.Phone} {o.Email}"))
                .Take(16)
                .Select(o =>
                {
                    var clinicName = clinicById.TryGetValue(o.ClinicId, out var clinic) ? clinic : "Nepoznata klinika";
                    var petCount = CountOrZero(patientCountsByOwner, o.Id);
                    return $"- {o.FirstName} {o.LastName} | klinika: {clinicName} | ljubimci: {petCount} | telefon: {o.Phone} | email: {o.Email}";
                })
                .ToList();

            var patientLines = patients
                .Where(p => Matches($"{p.Name} {p.Species} {p.Breed} {p.Color}"))
                .Take(16)
                .Select(p =>
                {
                    var ownerName = ownerById.TryGetValue(p.OwnerId, out var owner) ? owner : "Nepoznat vlasnik";
                    var appointmentCount = CountOrZero(appointmentCountsByPatient, p.Id);
                    var recordCount = CountOrZero(recordCountsByPatient, p.Id);
                    var attachmentCount = CountOrZero(attachmentCountsByPatient, p.Id);
                    return $"- {p.Name} | vrsta: {p.Species} | pasmina: {p.Breed} | boja: {p.Color} | vlasnik: {ownerName} | termini: {appointmentCount} | kartoni: {recordCount} | privitci: {attachmentCount}";
                })
                .ToList();

            var appointmentLines = appointments
                .Where(a => Matches($"{a.Reason} {a.Patient} {a.Vet} {a.Status}"))
                .Take(12)
                .Select(a =>
                {
                    var linkedServices = CountOrZero(appointmentServiceCountsByAppointment, a.Id);
                    return $"- {a.AppointmentDateTime:g} | razlog: {a.Reason} | pacijent: {a.Patient} | veterinar: {a.Vet} | status: {a.Status} | usluge: {linkedServices}";
                })
                .ToList();

            var recordLines = records
                .Where(r => Matches($"{r.Diagnosis} {r.Treatment} {r.Patient}"))
                .Take(12)
                .Select(r => $"- {r.RecordDate:d} | dijagnoza: {r.Diagnosis} | terapija: {r.Treatment} | pacijent: {r.Patient}")
                .ToList();

            var totalClinics = clinics.Count;
            var totalVets = vets.Count;
            var totalServices = services.Count;
            var totalOwners = owners.Count;
            var totalPatients = patients.Count;
            var totalAppointments = await _dbContext.Appointments.AsNoTracking().CountAsync(cancellationToken);
            var totalMedicalRecords = await _dbContext.MedicalRecords.AsNoTracking().CountAsync(cancellationToken);
            var totalAppointmentServices = await _dbContext.AppointmentServices.AsNoTracking().CountAsync(cancellationToken);
            var totalAttachments = await _dbContext.PatientAttachments.AsNoTracking().CountAsync(cancellationToken);

            var contextParts = new List<string>
            {
                "TOTALS (ALL PUBLIC CLASSES):",
                $"- Klinike: {totalClinics}",
                $"- Veterinari: {totalVets}",
                $"- Usluge: {totalServices}",
                $"- Vlasnici: {totalOwners}",
                $"- Pacijenti: {totalPatients}",
                $"- Termini: {totalAppointments}",
                $"- Medicinski zapisi: {totalMedicalRecords}",
                $"- AppointmentService veze: {totalAppointmentServices}",
                $"- Privitci pacijenata: {totalAttachments}",
                "",
                "PUBLIC CLINICS:",
                clinicLines.Count > 0 ? string.Join("\n", clinicLines) : "- Nema podataka za tražene pojmove.",
                "",
                "PUBLIC VETS:",
                vetLines.Count > 0 ? string.Join("\n", vetLines) : "- Nema podataka za tražene pojmove.",
                "",
                "PUBLIC SERVICES:",
                serviceLines.Count > 0 ? string.Join("\n", serviceLines) : "- Nema podataka za tražene pojmove.",
                "",
                "PUBLIC OWNERS:",
                ownerLines.Count > 0 ? string.Join("\n", ownerLines) : "- Nema podataka za tražene pojmove.",
                "",
                "PUBLIC PATIENTS:",
                patientLines.Count > 0 ? string.Join("\n", patientLines) : "- Nema podataka za tražene pojmove.",
                "",
                "RECENT APPOINTMENTS:",
                appointmentLines.Count > 0 ? string.Join("\n", appointmentLines) : "- Nema podataka za tražene pojmove.",
                "",
                "RECENT MEDICAL RECORDS:",
                recordLines.Count > 0 ? string.Join("\n", recordLines) : "- Nema podataka za tražene pojmove."
            };

            return string.Join("\n", contextParts);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to build public context for AI chat.");
            return "Public context is currently unavailable.";
        }
    }
}
