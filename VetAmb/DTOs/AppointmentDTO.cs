using System;
using System.Collections.Generic;

#nullable enable

namespace VetAmb.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? RescheduleReason { get; set; }
        public int PatientId { get; set; }
        public PatientDTO? Patient { get; set; }
        public int VetId { get; set; }
        public VetDTO? Vet { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public List<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
    }
}
