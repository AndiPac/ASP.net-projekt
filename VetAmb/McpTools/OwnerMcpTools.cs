using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for creating, retrieving, searching, updating, and soft-deleting pet owners.")]
public sealed class OwnerMcpTools
{
    private readonly IOwnerRepository _ownerRepository;

    public OwnerMcpTools(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }

    [McpServerTool]
    [Description("Create a new owner. Inputs: personal identity and contact fields, registration date, and clinic ID. Returns the created owner summary with patient count.")]
    public OwnerToolDto CreateOwner(
        [Description("Owner first name.")] string? firstName,
        [Description("Owner last name.")] string? lastName,
        [Description("Owner email address.")] string? email,
        [Description("Owner phone number.")] string? phone,
        [Description("Owner residential address.")] string? address,
        [Description("Date when the owner was first registered.")] DateTime registrationDate,
        [Description("Owner identification number used by the clinic.")] string? idNumber,
        [Description("Foreign key ID of the clinic that manages this owner.")] int clinicId)
    {
        var owner = new Owner
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Address = address,
            RegistrationDate = registrationDate,
            IdNumber = idNumber,
            ClinicId = clinicId
        };

        _ownerRepository.Add(owner);
        var created = _ownerRepository.GetById(owner.Id) ?? owner;
        return ToDto(created);
    }

    [McpServerTool]
    [Description("Get one owner by ID. Returns owner fields, clinic information, and number of linked patients. Throws an error if owner does not exist.")]
    public OwnerToolDto GetOwner(
        [Description("Primary key ID of the owner to retrieve.")] int id)
    {
        var owner = _ownerRepository.GetById(id)
            ?? throw new InvalidOperationException($"Owner with ID {id} was not found.");

        return ToDto(owner);
    }

    [McpServerTool]
    [Description("Search owners by term across first name, last name, email, phone, and ID number. Returns all non-deleted owners when search term is empty.")]
    public IReadOnlyList<OwnerToolDto> SearchOwners(
        [Description("Free-text search term.")] string? searchTerm)
    {
        var results = string.IsNullOrWhiteSpace(searchTerm)
            ? _ownerRepository.GetAll()
            : _ownerRepository.Search(searchTerm);

        return results.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Update an owner by ID. Any omitted field remains unchanged. Returns the updated owner summary.")]
    public OwnerToolDto UpdateOwner(
        [Description("Primary key ID of the owner to update.")] int id,
        [Description("Updated first name. Omit to keep current value.")] string? firstName = null,
        [Description("Updated last name. Omit to keep current value.")] string? lastName = null,
        [Description("Updated email. Omit to keep current value.")] string? email = null,
        [Description("Updated phone number. Omit to keep current value.")] string? phone = null,
        [Description("Updated address. Omit to keep current value.")] string? address = null,
        [Description("Updated registration date. Omit to keep current value.")] DateTime? registrationDate = null,
        [Description("Updated owner ID number. Omit to keep current value.")] string? idNumber = null,
        [Description("Updated clinic ID. Omit to keep current value.")] int? clinicId = null)
    {
        var owner = _ownerRepository.GetById(id)
            ?? throw new InvalidOperationException($"Owner with ID {id} was not found.");

        if (firstName is not null) owner.FirstName = firstName;
        if (lastName is not null) owner.LastName = lastName;
        if (email is not null) owner.Email = email;
        if (phone is not null) owner.Phone = phone;
        if (address is not null) owner.Address = address;
        if (registrationDate.HasValue) owner.RegistrationDate = registrationDate.Value;
        if (idNumber is not null) owner.IdNumber = idNumber;
        if (clinicId.HasValue) owner.ClinicId = clinicId.Value;

        _ownerRepository.Update(owner);
        return ToDto(owner);
    }

    [McpServerTool]
    [Description("Soft-delete an owner by ID so the record is hidden by query filters but retained in the database. Returns a status message.")]
    public string DeleteOwner(
        [Description("Primary key ID of the owner to soft-delete.")] int id)
    {
        var owner = _ownerRepository.GetById(id)
            ?? throw new InvalidOperationException($"Owner with ID {id} was not found.");

        _ownerRepository.SoftDelete(id);
        return $"Owner {owner.Id} soft-deleted successfully.";
    }

    private static OwnerToolDto ToDto(Owner owner)
    {
        return new OwnerToolDto(
            owner.Id,
            owner.FirstName,
            owner.LastName,
            owner.Email,
            owner.Phone,
            owner.Address,
            owner.RegistrationDate,
            owner.IdNumber,
            owner.ClinicId,
            owner.Clinic?.Name,
            owner.Patients.Count);
    }

    public sealed record OwnerToolDto(
        int Id,
        string? FirstName,
        string? LastName,
        string? Email,
        string? Phone,
        string? Address,
        DateTime RegistrationDate,
        string? IdNumber,
        int ClinicId,
        string? ClinicName,
        int PatientCount);
}
