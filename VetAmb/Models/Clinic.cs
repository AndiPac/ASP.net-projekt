using System;
using System.Collections.Generic;

#nullable enable

namespace VetAmb.Models
{
    /// <summary>
    /// Clinic class - represents a veterinary clinic (Complex class with 8 attributes)
    /// </summary>
    public class Clinic
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime FoundationDate { get; set; }
        public int MaxCapacity { get; set; }
        public string? RegistrationNumber { get; set; }

        // Relationships
        public ICollection<Vet> Vets { get; set; } = new List<Vet>();
        public ICollection<Owner> Owners { get; set; } = new List<Owner>();
    }
}
