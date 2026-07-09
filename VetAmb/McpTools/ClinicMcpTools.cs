using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for clinics. Read tools: GetClinic, SearchClinics. Write tools: CreateClinic, UpdateClinic, DeleteClinic.")]
public sealed class ClinicMcpTools
{
    private readonly IClinicRepository _clinicRepository;
    private readonly McpToolExecution _execution;

    public ClinicMcpTools(IClinicRepository clinicRepository, McpToolExecution execution)
    {
        _clinicRepository = clinicRepository;
        _execution = execution;
    }

    [McpServerTool]
    [Description("Create a new clinic record. Inputs: clinic name, address, phone, email, foundation date, max capacity, and registration number. Returns the created clinic including generated ID and summary relationship counts.")]
    public ClinicToolDto CreateClinic(
        [Description("Human-readable clinic name.")] string? name,
        [Description("Street address or location descriptor for the clinic.")] string? address,
        [Description("Primary phone number for the clinic.")] string? phone,
        [Description("Primary contact email for the clinic.")] string? email,
        [Description("Date the clinic was founded.")] DateTime foundationDate,
        [Description("Maximum number of patients the clinic can handle in active capacity.")] int maxCapacity,
        [Description("Internal or legal registration identifier for the clinic.")] string? registrationNumber)
    {
        return _execution.ExecuteWrite("Clinic.CreateClinic", () =>
        {
            var clinic = new Clinic
            {
                Name = name,
                Address = address,
                Phone = phone,
                Email = email,
                FoundationDate = foundationDate,
                MaxCapacity = maxCapacity,
                RegistrationNumber = registrationNumber
            };

            _clinicRepository.Add(clinic);
            var created = _clinicRepository.GetById(clinic.Id) ?? clinic;
            return ToDto(created);
        });
    }

    [McpServerTool]
    [Description("Get one clinic by numeric ID. Returns full clinic scalar fields plus counts of related vets and owners. Throws an error if no clinic exists with the provided ID.")]
    public ClinicToolDto GetClinic(
        [Description("Primary key ID of the clinic to retrieve.")] int id)
    {
        return _execution.ExecuteRead("Clinic.GetClinic", () =>
        {
            var clinic = _clinicRepository.GetById(id)
                ?? throw new InvalidOperationException($"Clinic with ID {id} was not found.");

            return ToDto(clinic);
        });
    }

    [McpServerTool]
    [Description("Search clinics by free-text term across name, address, and registration number. Returns a list of matching clinic summaries. Pass empty text to return all non-deleted clinics.")]
    public IReadOnlyList<ClinicToolDto> SearchClinics(
        [Description("Free-text filter term used against clinic name, address, and registration number.")] string? searchTerm)
    {
        return _execution.ExecuteRead("Clinic.SearchClinics", () =>
        {
            var results = string.IsNullOrWhiteSpace(searchTerm)
                ? _clinicRepository.GetAll()
                : _clinicRepository.Search(searchTerm);

            return results.Select(ToDto).ToList();
        });
    }

    [McpServerTool]
    [Description("Update an existing clinic by ID. Only provided fields are changed; omitted fields remain unchanged. Returns the updated clinic summary.")]
    public ClinicToolDto UpdateClinic(
        [Description("Primary key ID of the clinic to update.")] int id,
        [Description("New clinic name. Omit to keep current value.")] string? name = null,
        [Description("New clinic address. Omit to keep current value.")] string? address = null,
        [Description("New clinic phone. Omit to keep current value.")] string? phone = null,
        [Description("New clinic email. Omit to keep current value.")] string? email = null,
        [Description("New foundation date. Omit to keep current value.")] DateTime? foundationDate = null,
        [Description("New max capacity. Omit to keep current value.")] int? maxCapacity = null,
        [Description("New registration number. Omit to keep current value.")] string? registrationNumber = null)
    {
        return _execution.ExecuteWrite("Clinic.UpdateClinic", () =>
        {
            var clinic = _clinicRepository.GetById(id)
                ?? throw new InvalidOperationException($"Clinic with ID {id} was not found.");

            if (name is not null) clinic.Name = name;
            if (address is not null) clinic.Address = address;
            if (phone is not null) clinic.Phone = phone;
            if (email is not null) clinic.Email = email;
            if (foundationDate.HasValue) clinic.FoundationDate = foundationDate.Value;
            if (maxCapacity.HasValue) clinic.MaxCapacity = maxCapacity.Value;
            if (registrationNumber is not null) clinic.RegistrationNumber = registrationNumber;

            _clinicRepository.Update(clinic);
            return ToDto(clinic);
        });
    }

    [McpServerTool]
    [Description("Soft-delete a clinic by ID by setting DeletedAt in the data store. This does not physically remove the row. Returns an operation status message.")]
    public string DeleteClinic(
        [Description("Primary key ID of the clinic to soft-delete.")] int id)
    {
        return _execution.ExecuteWrite("Clinic.DeleteClinic", () =>
        {
            var clinic = _clinicRepository.GetById(id)
                ?? throw new InvalidOperationException($"Clinic with ID {id} was not found.");

            _clinicRepository.SoftDelete(id);
            return $"Clinic {clinic.Id} soft-deleted successfully.";
        });
    }

    private static ClinicToolDto ToDto(Clinic clinic)
    {
        return new ClinicToolDto(
            clinic.Id,
            clinic.Name,
            clinic.Address,
            clinic.Phone,
            clinic.Email,
            clinic.FoundationDate,
            clinic.MaxCapacity,
            clinic.RegistrationNumber,
            clinic.Vets.Count,
            clinic.Owners.Count);
    }

    public sealed record ClinicToolDto(
        int Id,
        string? Name,
        string? Address,
        string? Phone,
        string? Email,
        DateTime FoundationDate,
        int MaxCapacity,
        string? RegistrationNumber,
        int VetCount,
        int OwnerCount);
}
