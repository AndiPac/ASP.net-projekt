# VetAmb Sitemap & Route Configuration

## Application Navigation Structure

```
HOME (/)
│
├─ KLINIKE (/klinike)
│  ├─ Popis svih klinika
│  ├─ /klinike/{id} - Detalji klinike
│  └─ /klinike/{id}/uredu - Uređivanje klinike
│
├─ VLASNICI (/vlasnici)
│  ├─ Popis svih vlasnika
│  ├─ /vlasnici/{id} - Detalji vlasnika
│  ├─ /vlasnici/profil/{id} - Profil vlasnika (CUSTOM)
│  ├─ /vlasnici/{id}/uredu - Uređivanje vlasnika
│  └─ /vlasnici/novi - Dodavanje novog vlasnika
│
├─ PACIJENTI (/pacijenti)
│  ├─ Popis svih pacijenata (životinja)
│  ├─ /pacijenti/{id} - Detalji pacijenta
│  ├─ /pacijenti/{id}/uredu - Uređivanje pacijenta
│  └─ /pacijenti/novi - Dodavanje novog pacijenta
│
├─ VETERINARI (/veterinari)
│  ├─ Popis svih veterinara
│  ├─ /veterinari/{id} - Detalji veterinara
│  ├─ /veterinari/{id}/uredu - Uređivanje veterinara
│  └─ /veterinari/novi - Dodavanje novog veterinara
│
├─ TERMINI (/termini)
│  ├─ Popis svih termina
│  ├─ /termini/{id} - Detalji termina
│  ├─ /termini/novi - Dodavanje novog termina (CUSTOM)
│  ├─ /termini/{id}/uredu - Uređivanje termina
│  └─ /termini/{id}/otkaži - Otkazivanje termina
│
├─ USLUGE (/usluge)
│  ├─ Popis svih veterinarskih usluga
│  ├─ /usluge/{id} - Detalji usluge
│  ├─ /usluge/{id}/uredu - Uređivanje usluge
│  └─ /usluge/nova - Dodavanje nove usluge
│
└─ MEDICINSKA DOKUMENTACIJA (/medicinska-dokumentacija)
   ├─ Popis svih medicinski zapisa
   ├─ /medicinska-dokumentacija/{id} - Detalji zapisa
   ├─ /medicinska-dokumentacija/{id}/uredu - Uređivanje zapisa
   └─ /medicinska-dokumentacija/novi - Dodavanje novog zapisa
```

## Custom Route Definitions

### 1. Clinic Routes

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/klinike` | GET | ClinicController | Index | Popis svih klinika |
| `/klinike/{id}` | GET | ClinicController | Details | Detalji jedne klinike |
| `/klinike/{id}` | POST | ClinicController | Update | Ažuriranje klinike |
| `/klinike/{id}/uredu` | GET | ClinicController | Edit | Forma za uređivanje |
| `/klinike/nova` | GET | ClinicController | Create | Forma za dodavanje nove |

### 2. Owner Routes (CUSTOM)

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/vlasnici` | GET | OwnerController | Index | Popis svih vlasnika |
| `/vlasnici/{id}` | GET | OwnerController | Details | Detalji vlasnika |
| **`/vlasnici/profil/{id}`** | GET | OwnerController | Profile | **[CUSTOM]** Profil vlasnika s detaljima |
| `/vlasnici/{id}` | POST | OwnerController | Update | Ažuriranje vlasnika |
| `/vlasnici/{id}/uredu` | GET | OwnerController | Edit | Forma za uređivanje |
| `/vlasnici/novi` | GET | OwnerController | Create | Forma za dodavanje novog vlasnika |

### 3. Patient Routes

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/pacijenti` | GET | PatientController | Index | Popis svih pacijenata |
| `/pacijenti/{id}` | GET | PatientController | Details | Detalji pacijenta (životinje) |
| `/pacijenti/{id}/uredu` | GET | PatientController | Edit | Forma za uređivanje |
| `/pacijenti/novi` | GET | PatientController | Create | Forma za dodavanje novog pacijenta |

### 4. Vet Routes

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/veterinari` | GET | VetController | Index | Popis svih veterinara |
| `/veterinari/{id}` | GET | VetController | Details | Detalji veterinara |
| `/veterinari/{id}/uredu` | GET | VetController | Edit | Forma za uređivanje |
| `/veterinari/novi` | GET | VetController | Create | Forma za dodavanje novog veterinara |

### 5. Appointment Routes (CUSTOM)

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/termini` | GET | AppointmentController | Index | Popis svih termina |
| `/termini/{id}` | GET | AppointmentController | Details | Detalji termina |
| **`/termini/novi`** | GET | AppointmentController | Create | **[CUSTOM]** Forma za dodavanje novog termina |
| `/termini/{id}/uredu` | GET | AppointmentController | Edit | Forma za uređivanje termina |
| `/termini/{id}/otkaži` | POST | AppointmentController | Cancel | Otkazivanje termina |

### 6. Service Routes

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/usluge` | GET | ServiceController | Index | Popis svih usluga |
| `/usluge/{id}` | GET | ServiceController | Details | Detalji usluge |
| `/usluge/{id}/uredu` | GET | ServiceController | Edit | Forma za uređivanje |
| `/usluge/nova` | GET | ServiceController | Create | Forma za dodavanje nove usluge |

### 7. MedicalRecord Routes

| Route | HTTP Method | Controller | Action | Purpose |
|-------|------------|------------|--------|---------|
| `/medicinska-dokumentacija` | GET | MedicalRecordController | Index | Popis svih zapisa |
| `/medicinska-dokumentacija/{id}` | GET | MedicalRecordController | Details | Detalji zapisa |
| `/medicinska-dokumentacija/{id}/uredu` | GET | MedicalRecordController | Edit | Forma za uređivanje |
| `/medicinska-dokumentacija/novi` | GET | MedicalRecordController | Create | Forma za dodavanje novog zapisa |

## Custom Route Implementation Guide

### Custom Route 1: `/vlasnici/profil/{id}`
```csharp
// In OwnerController.cs
[Route("vlasnici/profil/{id}")]
public async Task<IActionResult> Profile(int id)
{
    // Prikazuje detaljni profil vlasnika s likom, kontaktima i listom njegovih pacijenata
    var owner = await _ownerRepository.GetOwnerByIdAsync(id);
    if (owner == null)
        return NotFound();
    return View(owner);
}
```

### Custom Route 2: `/termini/novi`
```csharp
// In AppointmentController.cs
[Route("termini/novi")]
[HttpGet]
public async Task<IActionResult> Create()
{
    // Prikazuje formu za dodavanje novog termina
    // Popunjava dropdowne s pacijentima i veterinarima
    ViewBag.Patients = await _patientRepository.GetAllPatientsAsync();
    ViewBag.Vets = await _vetRepository.GetAllVetsAsync();
    return View();
}

[Route("termini/novi")]
[HttpPost]
public async Task<IActionResult> Create(Appointment appointment)
{
    // Obrađuje kreiranje novog termina
    if (ModelState.IsValid)
    {
        await _appointmentRepository.CreateAppointmentAsync(appointment);
        return RedirectToAction("Index");
    }
    return View(appointment);
}
```

## Localized Route Configuration

- **English Fallback**: `/clinics`, `/owners`, `/patients`, `/vets`, `/appointments`, `/services`, `/medical-records`
- **Croatian Routes**: `/klinike`, `/vlasnici`, `/pacijenti`, `/veterinari`, `/termini`, `/usluge`, `/medicinska-dokumentacija`

## Route Prefixes

```csharp
// Custom route configuration in Program.cs or MapControllers()
builder.Services.AddControllersWithViews()
    .WithRoutes(options => {
        options.MapCultureRoute("{culture=en}/clinics", "ClinicController");
        options.MapCultureRoute("{culture=hr}/klinike", "ClinicController");
    });
```

## Navigation Menu Structure

```
Header Navigation:
- Početna (Home)
- Klinike (Clinics)
- Vlasnici (Owners)
- Pacijenti (Patients)
- Veterinari (Vets)
- Termini (Appointments)
  └─ [+ Novi termin]
- Usluge (Services)
- Medicinska dokumentacija (Medical Records)
- O nama (About)
- Kontakt (Contact)
```

## Breadcrumb Navigation

```
Primjeri breadcrumba:
- Početna > Vlasnici > Profil
- Početna > Termini > Novi termin
- Početna > Pacijenti > Detalji
- Početna > Veterinari > Uredu
```
