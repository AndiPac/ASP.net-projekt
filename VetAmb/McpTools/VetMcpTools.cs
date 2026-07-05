using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for creating, retrieving, searching, updating, and soft-deleting veterinarian records.")]
public sealed class VetMcpTools
{
    private readonly IVetRepository _vetRepository;

    public VetMcpTools(IVetRepository vetRepository)
    {
        _vetRepository = vetRepository;
    }

    [McpServerTool]
    [Description("Create a veterinarian. Inputs: first name, last name, specialization, license number, years of experience, phone, hourly rate, and clinic ID. Returns the newly created veterinarian summary.")]
    public VetToolDto CreateVet(
        [Description("Veterinarian first name.")] string? firstName,
        [Description("Veterinarian last name.")] string? lastName,
        [Description("Veterinary specialization enum value as text: GeneralPractice, Surgery, Dentistry, Cardiology, Dermatology, Orthopedics, InternalMedicine.")] string specialization,
        [Description("Professional license number.")] string? licenseNumber,
        [Description("Total years of veterinary practice.")] int yearsOfExperience,
        [Description("Primary contact phone number.")] string? phone,
        [Description("Hourly billing rate.")] decimal hourlyRate,
        [Description("Foreign key ID of the clinic where the veterinarian works.")] int clinicId)
    {
        var parsedSpecialization = ParseSpecialization(specialization);

        var vet = new Vet
        {
            FirstName = firstName,
            LastName = lastName,
            Specialization = parsedSpecialization,
            LicenseNumber = licenseNumber,
            YearsOfExperience = yearsOfExperience,
            Phone = phone,
            HourlyRate = hourlyRate,
            ClinicId = clinicId
        };

        _vetRepository.Add(vet);
        var created = _vetRepository.GetById(vet.Id) ?? vet;
        return ToDto(created);
    }

    [McpServerTool]
    [Description("Retrieve one veterinarian by ID. Returns identity, specialization, clinic link, and appointment count. Throws an error when ID does not exist.")]
    public VetToolDto GetVet(
        [Description("Primary key ID of the veterinarian to fetch.")] int id)
    {
        var vet = _vetRepository.GetById(id)
            ?? throw new InvalidOperationException($"Vet with ID {id} was not found.");

        return ToDto(vet);
    }

    [McpServerTool]
    [Description("Search veterinarians by term across first name, last name, license number, and phone. Returns all non-deleted vets when search term is omitted.")]
    public IReadOnlyList<VetToolDto> SearchVets(
        [Description("Free-text filter term.")] string? searchTerm)
    {
        var results = string.IsNullOrWhiteSpace(searchTerm)
            ? _vetRepository.GetAll()
            : _vetRepository.Search(searchTerm);

        return results.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Update an existing veterinarian by ID. Only provided arguments are applied. Returns the updated veterinarian summary.")]
    public VetToolDto UpdateVet(
        [Description("Primary key ID of the veterinarian to update.")] int id,
        [Description("Updated first name. Omit to keep existing value.")] string? firstName = null,
        [Description("Updated last name. Omit to keep existing value.")] string? lastName = null,
        [Description("Updated specialization text value. Omit to keep existing value.")] string? specialization = null,
        [Description("Updated license number. Omit to keep existing value.")] string? licenseNumber = null,
        [Description("Updated years of experience. Omit to keep existing value.")] int? yearsOfExperience = null,
        [Description("Updated phone number. Omit to keep existing value.")] string? phone = null,
        [Description("Updated hourly rate. Omit to keep existing value.")] decimal? hourlyRate = null,
        [Description("Updated clinic ID. Omit to keep existing value.")] int? clinicId = null)
    {
        var vet = _vetRepository.GetById(id)
            ?? throw new InvalidOperationException($"Vet with ID {id} was not found.");

        if (firstName is not null) vet.FirstName = firstName;
        if (lastName is not null) vet.LastName = lastName;
        if (specialization is not null) vet.Specialization = ParseSpecialization(specialization);
        if (licenseNumber is not null) vet.LicenseNumber = licenseNumber;
        if (yearsOfExperience.HasValue) vet.YearsOfExperience = yearsOfExperience.Value;
        if (phone is not null) vet.Phone = phone;
        if (hourlyRate.HasValue) vet.HourlyRate = hourlyRate.Value;
        if (clinicId.HasValue) vet.ClinicId = clinicId.Value;

        _vetRepository.Update(vet);
        return ToDto(vet);
    }

    [McpServerTool]
    [Description("Soft-delete a veterinarian by ID. The row is retained but excluded from normal queries. Returns a status message.")]
    public string DeleteVet(
        [Description("Primary key ID of the veterinarian to soft-delete.")] int id)
    {
        var vet = _vetRepository.GetById(id)
            ?? throw new InvalidOperationException($"Vet with ID {id} was not found.");

        _vetRepository.SoftDelete(id);
        return $"Vet {vet.Id} soft-deleted successfully.";
    }

    private static VeterinarySpecialization ParseSpecialization(string specialization)
    {
        if (Enum.TryParse<VeterinarySpecialization>(specialization, true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException(
            "Invalid specialization. Allowed values: GeneralPractice, Surgery, Dentistry, Cardiology, Dermatology, Orthopedics, InternalMedicine.",
            nameof(specialization));
    }

    private static VetToolDto ToDto(Vet vet)
    {
        return new VetToolDto(
            vet.Id,
            vet.FirstName,
            vet.LastName,
            vet.Specialization.ToString(),
            vet.LicenseNumber,
            vet.YearsOfExperience,
            vet.Phone,
            vet.HourlyRate,
            vet.ClinicId,
            vet.Clinic?.Name,
            vet.Appointments.Count);
    }

    public sealed record VetToolDto(
        int Id,
        string? FirstName,
        string? LastName,
        string Specialization,
        string? LicenseNumber,
        int YearsOfExperience,
        string? Phone,
        decimal HourlyRate,
        int ClinicId,
        string? ClinicName,
        int AppointmentCount);
}
