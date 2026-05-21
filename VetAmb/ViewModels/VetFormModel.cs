using System.ComponentModel.DataAnnotations;
using VetAmb.Models;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Vet Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt and navigation properties are intentionally excluded.
    /// ClinicId is required (FK) and is bound via AJAX autocomplete.
    /// </summary>
    public class VetFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime veterinara je obavezno.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Prezime veterinara je obavezno.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Specijalizacija je obavezna.")]
        public VeterinarySpecialization Specialization { get; set; }

        [Required(ErrorMessage = "Broj licence je obavezan.")]
        [StringLength(50, ErrorMessage = "Broj licence ne smije biti dulji od 50 znakova.")]
        public string? LicenseNumber { get; set; }

        [Required(ErrorMessage = "Godine iskustva su obavezne.")]
        [Range(0, 60, ErrorMessage = "Godine iskustva moraju biti između 0 i 60.")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [StringLength(30, ErrorMessage = "Broj telefona ne smije biti dulji od 30 znakova.")]
        [RegularExpression(@"^[\+]?[\d\s\-\(\)\.]{6,30}$",
            ErrorMessage = "Unesite ispravan broj telefona (npr. 091 234 5678 ili +385 91 234 5678).")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Satnica je obavezna.")]
        [Range(0.01, 9999.99, ErrorMessage = "Satnica mora biti između 0.01 € i 9999.99 €.")]
        public decimal HourlyRate { get; set; }

        /// <summary>
        /// FK to Clinic — required. Bound via AJAX autocomplete; validated server- and client-side.
        /// </summary>
        [Required(ErrorMessage = "Odabir klinike je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite kliniku s popisa.")]
        public int ClinicId { get; set; }
    }
}
