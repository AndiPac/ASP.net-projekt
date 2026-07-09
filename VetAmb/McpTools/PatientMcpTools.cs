using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for patients. Read tools: GetPatient, SearchPatients. Write tools: CreatePatient, UpdatePatient, DeletePatient.")]
public sealed class PatientMcpTools
{
    private readonly IPatientRepository _patientRepository;
    private readonly McpToolExecution _execution;

    public PatientMcpTools(IPatientRepository patientRepository, McpToolExecution execution)
    {
        _patientRepository = patientRepository;
        _execution = execution;
    }

    [McpServerTool]
    [Description("Create a patient (animal) record. Inputs include identity fields, species, demographics, and owner ID. Returns the created patient summary.")]
    public PatientToolDto CreatePatient(
        [Description("Patient or pet name.")] string? name,
        [Description("Species enum value as text: Dog, Cat, Bird, Rabbit, Hamster, Guinea_Pig, Ferret, Reptile.")] string species,
        [Description("Breed or subtype.")] string? breed,
        [Description("Date of birth of the animal.")] DateTime dateOfBirth,
        [Description("Current weight in kilograms.")] decimal weight,
        [Description("Microchip identifier.")] string? microchipId,
        [Description("Primary coat/skin color descriptor.")] string? color,
        [Description("Foreign key ID of the owner.")] int ownerId)
    {
        return _execution.ExecuteWrite("Patient.CreatePatient", () =>
        {
            var parsedSpecies = ParseSpecies(species);

            var patient = new Patient
            {
                Name = name,
                Species = parsedSpecies,
                Breed = breed,
                DateOfBirth = dateOfBirth,
                Weight = weight,
                MicrochipId = microchipId,
                Color = color,
                OwnerId = ownerId
            };

            _patientRepository.Add(patient);
            var created = _patientRepository.GetById(patient.Id) ?? patient;
            return ToDto(created);
        });
    }

    [McpServerTool]
    [Description("Get one patient by ID. Returns patient fields, owner summary, appointment count, and medical record count. Throws an error if ID is not found.")]
    public PatientToolDto GetPatient(
        [Description("Primary key ID of the patient to fetch.")] int id)
    {
        return _execution.ExecuteRead("Patient.GetPatient", () =>
        {
            var patient = _patientRepository.GetById(id)
                ?? throw new InvalidOperationException($"Patient with ID {id} was not found.");

            return ToDto(patient);
        });
    }

    [McpServerTool]
    [Description("Search patients by term across patient name, breed, and microchip identifier. Returns all non-deleted patients when term is empty.")]
    public IReadOnlyList<PatientToolDto> SearchPatients(
        [Description("Free-text search term.")] string? searchTerm)
    {
        return _execution.ExecuteRead("Patient.SearchPatients", () =>
        {
            var results = string.IsNullOrWhiteSpace(searchTerm)
                ? _patientRepository.GetAll()
                : _patientRepository.Search(searchTerm);

            return results.Select(ToDto).ToList();
        });
    }

    [McpServerTool]
    [Description("Update an existing patient by ID. Only provided fields are changed; omitted fields remain unchanged. Returns the updated patient summary.")]
    public PatientToolDto UpdatePatient(
        [Description("Primary key ID of the patient to update.")] int id,
        [Description("Updated patient name. Omit to keep current value.")] string? name = null,
        [Description("Updated species enum value as text. Omit to keep current value.")] string? species = null,
        [Description("Updated breed. Omit to keep current value.")] string? breed = null,
        [Description("Updated date of birth. Omit to keep current value.")] DateTime? dateOfBirth = null,
        [Description("Updated weight in kilograms. Omit to keep current value.")] decimal? weight = null,
        [Description("Updated microchip ID. Omit to keep current value.")] string? microchipId = null,
        [Description("Updated color. Omit to keep current value.")] string? color = null,
        [Description("Updated owner ID. Omit to keep current value.")] int? ownerId = null)
    {
        return _execution.ExecuteWrite("Patient.UpdatePatient", () =>
        {
            var patient = _patientRepository.GetById(id)
                ?? throw new InvalidOperationException($"Patient with ID {id} was not found.");

            if (name is not null) patient.Name = name;
            if (species is not null) patient.Species = ParseSpecies(species);
            if (breed is not null) patient.Breed = breed;
            if (dateOfBirth.HasValue) patient.DateOfBirth = dateOfBirth.Value;
            if (weight.HasValue) patient.Weight = weight.Value;
            if (microchipId is not null) patient.MicrochipId = microchipId;
            if (color is not null) patient.Color = color;
            if (ownerId.HasValue) patient.OwnerId = ownerId.Value;

            _patientRepository.Update(patient);
            return ToDto(patient);
        });
    }

    [McpServerTool]
    [Description("Soft-delete a patient by ID. Record remains in storage with DeletedAt set, and is hidden from normal queries. Returns operation status text.")]
    public string DeletePatient(
        [Description("Primary key ID of the patient to soft-delete.")] int id)
    {
        return _execution.ExecuteWrite("Patient.DeletePatient", () =>
        {
            var patient = _patientRepository.GetById(id)
                ?? throw new InvalidOperationException($"Patient with ID {id} was not found.");

            _patientRepository.SoftDelete(id);
            return $"Patient {patient.Id} soft-deleted successfully.";
        });
    }

    private static AnimalSpecies ParseSpecies(string species)
    {
        if (Enum.TryParse<AnimalSpecies>(species, true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException(
            "Invalid species. Allowed values: Dog, Cat, Bird, Rabbit, Hamster, Guinea_Pig, Ferret, Reptile.",
            nameof(species));
    }

    private static PatientToolDto ToDto(Patient patient)
    {
        return new PatientToolDto(
            patient.Id,
            patient.Name,
            patient.Species.ToString(),
            patient.Breed,
            patient.DateOfBirth,
            patient.Weight,
            patient.MicrochipId,
            patient.Color,
            patient.OwnerId,
            patient.Owner is null ? null : $"{patient.Owner.FirstName} {patient.Owner.LastName}".Trim(),
            patient.Appointments.Count,
            patient.MedicalRecords.Count);
    }

    public sealed record PatientToolDto(
        int Id,
        string? Name,
        string Species,
        string? Breed,
        DateTime DateOfBirth,
        decimal Weight,
        string? MicrochipId,
        string? Color,
        int OwnerId,
        string? OwnerName,
        int AppointmentCount,
        int MedicalRecordCount);
}
