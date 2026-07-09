using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for medical records. Read tools: GetMedicalRecord, SearchMedicalRecords. Write tools: CreateMedicalRecord, UpdateMedicalRecord, DeleteMedicalRecord.")]
public sealed class MedicalRecordMcpTools
{
    private readonly IMedicalRecordRepository _medicalRecordRepository;
    private readonly McpToolExecution _execution;

    public MedicalRecordMcpTools(IMedicalRecordRepository medicalRecordRepository, McpToolExecution execution)
    {
        _medicalRecordRepository = medicalRecordRepository;
        _execution = execution;
    }

    [McpServerTool]
    [Description("Create a medical record linked to a patient. Inputs: diagnosis, treatment, record date, notes, and patient ID. Returns the created medical record summary.")]
    public MedicalRecordToolDto CreateMedicalRecord(
        [Description("Diagnosis or clinical assessment text.")] string? diagnosis,
        [Description("Treatment plan or performed intervention.")] string? treatment,
        [Description("Date of this medical record entry.")] DateTime recordDate,
        [Description("Additional clinician notes.")] string? notes,
        [Description("Foreign key ID of the patient this record belongs to.")] int patientId)
    {
        return _execution.ExecuteWrite("MedicalRecord.CreateMedicalRecord", () =>
        {
            var record = new MedicalRecord
            {
                Diagnosis = diagnosis,
                Treatment = treatment,
                RecordDate = recordDate,
                Notes = notes,
                PatientId = patientId
            };

            _medicalRecordRepository.Add(record);
            var created = _medicalRecordRepository.GetById(record.Id) ?? record;
            return ToDto(created);
        });
    }

    [McpServerTool]
    [Description("Get one medical record by ID. Returns diagnosis and linkage information. Throws an error if no record exists for the provided ID.")]
    public MedicalRecordToolDto GetMedicalRecord(
        [Description("Primary key ID of the medical record to retrieve.")] int id)
    {
        return _execution.ExecuteRead("MedicalRecord.GetMedicalRecord", () =>
        {
            var record = _medicalRecordRepository.GetById(id)
                ?? throw new InvalidOperationException($"Medical record with ID {id} was not found.");

            return ToDto(record);
        });
    }

    [McpServerTool]
    [Description("Search medical records by diagnosis, treatment, notes, or patient name. Returns all non-deleted records when search term is empty.")]
    public IReadOnlyList<MedicalRecordToolDto> SearchMedicalRecords(
        [Description("Free-text term for filtering medical records.")] string? searchTerm)
    {
        return _execution.ExecuteRead("MedicalRecord.SearchMedicalRecords", () =>
        {
            var results = string.IsNullOrWhiteSpace(searchTerm)
                ? _medicalRecordRepository.GetAll()
                : _medicalRecordRepository.Search(searchTerm);

            return results.Select(ToDto).ToList();
        });
    }

    [McpServerTool]
    [Description("Update one medical record by ID. Only supplied fields are changed. Returns the updated medical record summary.")]
    public MedicalRecordToolDto UpdateMedicalRecord(
        [Description("Primary key ID of the medical record to update.")] int id,
        [Description("Updated diagnosis text. Omit to keep current value.")] string? diagnosis = null,
        [Description("Updated treatment text. Omit to keep current value.")] string? treatment = null,
        [Description("Updated record date. Omit to keep current value.")] DateTime? recordDate = null,
        [Description("Updated notes text. Omit to keep current value.")] string? notes = null,
        [Description("Updated patient ID. Omit to keep current value.")] int? patientId = null)
    {
        return _execution.ExecuteWrite("MedicalRecord.UpdateMedicalRecord", () =>
        {
            var record = _medicalRecordRepository.GetById(id)
                ?? throw new InvalidOperationException($"Medical record with ID {id} was not found.");

            if (diagnosis is not null) record.Diagnosis = diagnosis;
            if (treatment is not null) record.Treatment = treatment;
            if (recordDate.HasValue) record.RecordDate = recordDate.Value;
            if (notes is not null) record.Notes = notes;
            if (patientId.HasValue) record.PatientId = patientId.Value;

            _medicalRecordRepository.Update(record);
            return ToDto(record);
        });
    }

    [McpServerTool]
    [Description("Soft-delete a medical record by ID. Record is retained with DeletedAt set and excluded from normal queries. Returns operation status text.")]
    public string DeleteMedicalRecord(
        [Description("Primary key ID of the medical record to soft-delete.")] int id)
    {
        return _execution.ExecuteWrite("MedicalRecord.DeleteMedicalRecord", () =>
        {
            var record = _medicalRecordRepository.GetById(id)
                ?? throw new InvalidOperationException($"Medical record with ID {id} was not found.");

            _medicalRecordRepository.SoftDelete(id);
            return $"Medical record {record.Id} soft-deleted successfully.";
        });
    }

    private static MedicalRecordToolDto ToDto(MedicalRecord record)
    {
        return new MedicalRecordToolDto(
            record.Id,
            record.Diagnosis,
            record.Treatment,
            record.RecordDate,
            record.Notes,
            record.PatientId,
            record.Patient?.Name);
    }

    public sealed record MedicalRecordToolDto(
        int Id,
        string? Diagnosis,
        string? Treatment,
        DateTime RecordDate,
        string? Notes,
        int PatientId,
        string? PatientName);
}
