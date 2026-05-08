# VetAmb Semantic Model

## Overview
VetAmb je veterinarski informacijski sustav koji upravlja klinikama, veterinarima, vlasnicima kućnih ljubimaca, pacijentima (životinjama), terminama, medicinskom dokumentacijom i uslugama.

## Entity Relationships Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                       CLINIC                                │
│  (Id, Name, Address, Phone, Email,                          │
│   FoundationDate, MaxCapacity, RegistrationNumber)          │
└──────────────────┬────────────────────────────────────┬─────┘
                   │ 1                              1   │
                   │ N                              N   │
        ┌──────────┴──────────┐           ┌─────────────┴────────────┐
        │                     │           │                          │
    ┌───▼─────────┐      ┌───▼─────────┐ │
    │     VET     │      │    OWNER    │ │
    │ (Id, Name,  │      │ (Id, Name,  │ │
    │  License,   │      │  Email,     │ │
    │  Phone,     │      │  Phone,     │ │
    │  Hourly,    │      │  IdNumber,  │ │
    │  Experience)│      │  RegDate)   │ │
    └───┬─────────┘      └───┬─────────┘ │
        │ 1                  │ 1          │
        │ N                  │ N          │
        │                    └────────────┼──────────────┐
        │                                 │              │
    ┌───▼────────────────┐          ┌────▼────────┐     │
    │   APPOINTMENT      │          │   PATIENT   │     │
    │ (Id, DateTime,     │◄────────►│ (Id, Name,  │     │
    │  Reason, Status,   │  1   N   │  Species,   │     │
    │  Notes)            │          │  Breed,     │     │
    │                    │          │  Weight,    │     │
    │  Foreign Keys:     │          │  Color,     │     │
    │  - PatientId       │          │  Microchip) │     │
    │  - VetId           │          └────┬────────┘     │
    └────────┬───────────┘               │ 1            │
             │                           │ N            │
             │ N                         │              │
             │ M (via junction)          └──────────────┘
             │
        ┌────▼──────────────────┐
        │ APPOINTMENTSERVICE     │
        │ (Id, AppointmentId,    │
        │  ServiceId)            │
        │ [Junction Table]       │
        └────┬──────────────────┘
             │
             │ N
             │ M
        ┌────▼────────────┐
        │    SERVICE      │
        │ (Id, Name,      │
        │  Description,   │
        │  Price, Duration)
        └─────────────────┘

        ┌──────────────────┐
        │ MEDICALRECORD    │
        │ (Id, Diagnosis,  │
        │  Treatment,      │
        │  RecordDate,     │
        │  Notes)          │
        └────────┬─────────┘
                 │ N
                 │ belongs to
             ┌───▼─────────────┐
             │    PATIENT      │
             │   (Patient 1:N  │
             │    Relationship)│
             └─────────────────┘
```

## Detailed Entity Descriptions

### 1. **CLINIC** (1:N relationships)
- **Attributes**: Id, Name, Address, Phone, Email, FoundationDate, MaxCapacity, RegistrationNumber
- **Relationships**:
  - 1 Clinic → N Vets (veterinari u klinici)
  - 1 Clinic → N Owners (vlasnici registrirani u klinici)
- **Primary Key**: Id

### 2. **VET** (Veterinarian)
- **Attributes**: Id, FirstName, LastName, Specialization (enum), LicenseNumber, YearsOfExperience, Phone, HourlyRate
- **Foreign Keys**: ClinicId (references Clinic)
- **Relationships**:
  - N Vets → 1 Clinic
  - 1 Vet → N Appointments
- **Primary Key**: Id

### 3. **OWNER** (Pet Owner)
- **Attributes**: Id, FirstName, LastName, Email, Phone, Address, RegistrationDate, IdNumber
- **Foreign Keys**: ClinicId (references Clinic)
- **Relationships**:
  - N Owners → 1 Clinic
  - 1 Owner → N Patients
- **Primary Key**: Id

### 4. **PATIENT** (Pet)
- **Attributes**: Id, Name, Species (enum: AnimalSpecies), Breed, DateOfBirth, Weight, MicrochipId, Color
- **Foreign Keys**: OwnerId (references Owner)
- **Relationships**:
  - N Patients → 1 Owner
  - 1 Patient → N Appointments
  - 1 Patient → N MedicalRecords
- **Primary Key**: Id

### 5. **APPOINTMENT** (Veterinary Appointment)
- **Attributes**: Id, AppointmentDateTime, Reason, Status (enum: AppointmentStatus), Notes, RescheduleReason
- **Foreign Keys**: PatientId (references Patient), VetId (references Vet)
- **Relationships**:
  - N Appointments → 1 Patient
  - N Appointments → 1 Vet
  - N:M Appointments ← AppointmentService → Services
- **Primary Key**: Id

### 6. **SERVICE** (Veterinary Service)
- **Attributes**: Id, Name, Description, Price, EstimatedDurationMinutes
- **Relationships**:
  - N:M Services ← AppointmentService → Appointments
- **Primary Key**: Id

### 7. **APPOINTMENTSERVICE** (Junction Table)
- **Attributes**: Id, AppointmentId, ServiceId
- **Foreign Keys**: AppointmentId (references Appointment), ServiceId (references Service)
- **Purpose**: Modelira N:M relaciju između Appointment i Service (jedan termin može imati više usluga)
- **Primary Key**: Id
- **Composite Unique Key**: (AppointmentId, ServiceId)

### 8. **MEDICALRECORD** (Medical History)
- **Attributes**: Id, Diagnosis, Treatment, RecordDate, Notes
- **Foreign Keys**: PatientId (references Patient)
- **Relationships**:
  - N MedicalRecords → 1 Patient
- **Primary Key**: Id

## Relationship Summary

| From Entity | To Entity | Type | Cardinality |
|-------------|-----------|------|-------------|
| Clinic | Vet | Foreign Key | 1:N |
| Clinic | Owner | Foreign Key | 1:N |
| Owner | Patient | Foreign Key | 1:N |
| Patient | Appointment | Foreign Key | 1:N |
| Patient | MedicalRecord | Foreign Key | 1:N |
| Vet | Appointment | Foreign Key | 1:N |
| Appointment | Service | Junction Table (AppointmentService) | N:M |

## Namespace Hierarchy

```
VetAmb.Models
├── Clinic
├── Owner
├── Patient
├── Vet
├── Appointment
├── Service
├── MedicalRecord
├── AppointmentService
├── Enums
│   ├── AnimalSpecies
│   ├── VeterinarySpecialization
│   └── AppointmentStatus
└── Services
    └── AppointmentService (Business Logic)
```

## Key Design Patterns

1. **Nullable Reference Types** (#nullable enable) - Koristi se za sigurnost pri nultim vrijednostima
2. **Navigation Properties** - Omogućavaju lakšu navigaciju između entiteta
3. **Foreign Key Conventions** - Koristi se property naming convention (EntityId)
4. **Junction Table** - AppointmentService omogućava fleksibilnu N:M relaciju
5. **Enumerations** - VeterinarySpecialization, AnimalSpecies, AppointmentStatus za kategorijalizaciju
