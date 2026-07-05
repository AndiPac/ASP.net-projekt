using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using VetAmb.Data;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for creating, retrieving, searching, updating, and soft-deleting appointments, including linked service IDs.")]
public sealed class AppointmentMcpTools
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly VetAmbDbContext _dbContext;

    public AppointmentMcpTools(IAppointmentRepository appointmentRepository, VetAmbDbContext dbContext)
    {
        _appointmentRepository = appointmentRepository;
        _dbContext = dbContext;
    }

    [McpServerTool]
    [Description("Create an appointment. Inputs: date/time, reason, status enum text, notes, reschedule reason, patient ID, vet ID, and optional service IDs. Returns the created appointment summary with linked services.")]
    public AppointmentToolDto CreateAppointment(
        [Description("Date and time when the appointment is scheduled.")] DateTime appointmentDateTime,
        [Description("Primary appointment reason.")] string? reason,
        [Description("Appointment status enum text: Scheduled, InProgress, Completed, Cancelled, NoShow, Rescheduled.")] string status,
        [Description("Optional clinical notes.")] string? notes,
        [Description("Optional reason for rescheduling if status is Rescheduled.")] string? rescheduleReason,
        [Description("Foreign key ID of the patient.")] int patientId,
        [Description("Foreign key ID of the veterinarian.")] int vetId,
        [Description("Optional list of service IDs to attach to this appointment.")] int[]? serviceIds = null)
    {
        var parsedStatus = ParseStatus(status);

        var appointment = new Appointment
        {
            AppointmentDateTime = appointmentDateTime,
            Reason = reason,
            Status = parsedStatus,
            Notes = notes,
            RescheduleReason = rescheduleReason,
            PatientId = patientId,
            VetId = vetId
        };

        _appointmentRepository.Add(appointment);

        if (serviceIds is { Length: > 0 })
        {
            var distinctServiceIds = serviceIds.Distinct().ToArray();
            foreach (var serviceId in distinctServiceIds)
            {
                _dbContext.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.Id,
                    ServiceId = serviceId
                });
            }

            _dbContext.SaveChanges();
        }

        var created = _appointmentRepository.GetById(appointment.Id) ?? appointment;
        return ToDto(created);
    }

    [McpServerTool]
    [Description("Get one appointment by ID. Returns appointment fields plus patient, vet, and linked service IDs. Throws an error if not found.")]
    public AppointmentToolDto GetAppointment(
        [Description("Primary key ID of the appointment to fetch.")] int id)
    {
        var appointment = _appointmentRepository.GetById(id)
            ?? throw new InvalidOperationException($"Appointment with ID {id} was not found.");

        return ToDto(appointment);
    }

    [McpServerTool]
    [Description("Search appointments by reason, notes, reschedule reason, patient name, or vet name. Returns all non-deleted appointments when term is empty.")]
    public IReadOnlyList<AppointmentToolDto> SearchAppointments(
        [Description("Free-text filter term for appointment search.")] string? searchTerm)
    {
        var results = string.IsNullOrWhiteSpace(searchTerm)
            ? _appointmentRepository.GetAll()
            : _appointmentRepository.Search(searchTerm);

        return results.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Update an appointment by ID. Any omitted argument stays unchanged. If serviceIds is provided, linked services are replaced exactly by that list. Returns updated appointment summary.")]
    public AppointmentToolDto UpdateAppointment(
        [Description("Primary key ID of the appointment to update.")] int id,
        [Description("Updated date/time. Omit to keep current value.")] DateTime? appointmentDateTime = null,
        [Description("Updated reason. Omit to keep current value.")] string? reason = null,
        [Description("Updated status enum text. Omit to keep current value.")] string? status = null,
        [Description("Updated notes. Omit to keep current value.")] string? notes = null,
        [Description("Updated reschedule reason. Omit to keep current value.")] string? rescheduleReason = null,
        [Description("Updated patient ID. Omit to keep current value.")] int? patientId = null,
        [Description("Updated vet ID. Omit to keep current value.")] int? vetId = null,
        [Description("Optional full replacement list of linked service IDs.")] int[]? serviceIds = null)
    {
        var appointment = _appointmentRepository.GetById(id)
            ?? throw new InvalidOperationException($"Appointment with ID {id} was not found.");

        if (appointmentDateTime.HasValue) appointment.AppointmentDateTime = appointmentDateTime.Value;
        if (reason is not null) appointment.Reason = reason;
        if (status is not null) appointment.Status = ParseStatus(status);
        if (notes is not null) appointment.Notes = notes;
        if (rescheduleReason is not null) appointment.RescheduleReason = rescheduleReason;
        if (patientId.HasValue) appointment.PatientId = patientId.Value;
        if (vetId.HasValue) appointment.VetId = vetId.Value;

        _appointmentRepository.Update(appointment);

        if (serviceIds is not null)
        {
            var existing = _dbContext.AppointmentServices
                .Where(x => x.AppointmentId == appointment.Id)
                .ToList();

            if (existing.Count > 0)
            {
                _dbContext.AppointmentServices.RemoveRange(existing);
            }

            var distinctServiceIds = serviceIds.Distinct().ToArray();
            foreach (var serviceId in distinctServiceIds)
            {
                _dbContext.AppointmentServices.Add(new AppointmentService
                {
                    AppointmentId = appointment.Id,
                    ServiceId = serviceId
                });
            }

            _dbContext.SaveChanges();
            appointment = _appointmentRepository.GetById(id) ?? appointment;
        }

        return ToDto(appointment);
    }

    [McpServerTool]
    [Description("Soft-delete an appointment by ID by setting DeletedAt. Returns a status message.")]
    public string DeleteAppointment(
        [Description("Primary key ID of the appointment to soft-delete.")] int id)
    {
        var appointment = _appointmentRepository.GetById(id)
            ?? throw new InvalidOperationException($"Appointment with ID {id} was not found.");

        _appointmentRepository.SoftDelete(id);
        return $"Appointment {appointment.Id} soft-deleted successfully.";
    }

    private static AppointmentStatus ParseStatus(string status)
    {
        if (Enum.TryParse<AppointmentStatus>(status, true, out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException(
            "Invalid appointment status. Allowed values: Scheduled, InProgress, Completed, Cancelled, NoShow, Rescheduled.",
            nameof(status));
    }

    private static AppointmentToolDto ToDto(Appointment appointment)
    {
        return new AppointmentToolDto(
            appointment.Id,
            appointment.AppointmentDateTime,
            appointment.Reason,
            appointment.Status.ToString(),
            appointment.Notes,
            appointment.RescheduleReason,
            appointment.PatientId,
            appointment.Patient?.Name,
            appointment.VetId,
            appointment.Vet is null ? null : $"{appointment.Vet.FirstName} {appointment.Vet.LastName}".Trim(),
            appointment.AppointmentServices.Select(x => x.ServiceId).OrderBy(x => x).ToArray());
    }

    public sealed record AppointmentToolDto(
        int Id,
        DateTime AppointmentDateTime,
        string? Reason,
        string Status,
        string? Notes,
        string? RescheduleReason,
        int PatientId,
        string? PatientName,
        int VetId,
        string? VetName,
        int[] ServiceIds);
}
