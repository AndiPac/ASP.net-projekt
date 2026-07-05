using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ModelContextProtocol.Server;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.McpTools;

[McpServerToolType]
[Description("MCP tools for creating, retrieving, searching, updating, and soft-deleting veterinary services.")]
public sealed class ServiceMcpTools
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceMcpTools(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    [McpServerTool]
    [Description("Create a service offering. Inputs: name, description, price, and estimated duration in minutes. Returns the created service summary.")]
    public ServiceToolDto CreateService(
        [Description("Service name displayed to users and staff.")] string? name,
        [Description("Detailed service description.")] string? description,
        [Description("Service price.")] decimal price,
        [Description("Estimated duration in minutes.")] int estimatedDurationMinutes)
    {
        var service = new Service
        {
            Name = name,
            Description = description,
            Price = price,
            EstimatedDurationMinutes = estimatedDurationMinutes
        };

        _serviceRepository.Add(service);
        var created = _serviceRepository.GetById(service.Id) ?? service;
        return ToDto(created);
    }

    [McpServerTool]
    [Description("Get one service by ID. Returns service details and appointment usage count. Throws an error if no service exists for the provided ID.")]
    public ServiceToolDto GetService(
        [Description("Primary key ID of the service to fetch.")] int id)
    {
        var service = _serviceRepository.GetById(id)
            ?? throw new InvalidOperationException($"Service with ID {id} was not found.");

        return ToDto(service);
    }

    [McpServerTool]
    [Description("Search services by name or description text. Returns all non-deleted services when search term is empty.")]
    public IReadOnlyList<ServiceToolDto> SearchServices(
        [Description("Free-text search term for service matching.")] string? searchTerm)
    {
        var results = string.IsNullOrWhiteSpace(searchTerm)
            ? _serviceRepository.GetAll()
            : _serviceRepository.Search(searchTerm);

        return results.Select(ToDto).ToList();
    }

    [McpServerTool]
    [Description("Update one service by ID. Any omitted argument remains unchanged. Returns the updated service summary.")]
    public ServiceToolDto UpdateService(
        [Description("Primary key ID of the service to update.")] int id,
        [Description("Updated name. Omit to keep current value.")] string? name = null,
        [Description("Updated description. Omit to keep current value.")] string? description = null,
        [Description("Updated price. Omit to keep current value.")] decimal? price = null,
        [Description("Updated estimated duration in minutes. Omit to keep current value.")] int? estimatedDurationMinutes = null)
    {
        var service = _serviceRepository.GetById(id)
            ?? throw new InvalidOperationException($"Service with ID {id} was not found.");

        if (name is not null) service.Name = name;
        if (description is not null) service.Description = description;
        if (price.HasValue) service.Price = price.Value;
        if (estimatedDurationMinutes.HasValue) service.EstimatedDurationMinutes = estimatedDurationMinutes.Value;

        _serviceRepository.Update(service);
        return ToDto(service);
    }

    [McpServerTool]
    [Description("Soft-delete a service by ID. The row remains stored but is excluded by query filters. Returns operation status text.")]
    public string DeleteService(
        [Description("Primary key ID of the service to soft-delete.")] int id)
    {
        var service = _serviceRepository.GetById(id)
            ?? throw new InvalidOperationException($"Service with ID {id} was not found.");

        _serviceRepository.SoftDelete(id);
        return $"Service {service.Id} soft-deleted successfully.";
    }

    private static ServiceToolDto ToDto(Service service)
    {
        return new ServiceToolDto(
            service.Id,
            service.Name,
            service.Description,
            service.Price,
            service.EstimatedDurationMinutes,
            service.AppointmentServices.Count);
    }

    public sealed record ServiceToolDto(
        int Id,
        string? Name,
        string? Description,
        decimal Price,
        int EstimatedDurationMinutes,
        int AppointmentUsageCount);
}
