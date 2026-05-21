using System.ComponentModel.DataAnnotations;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Clinic Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt, FoundationDate, MaxCapacity and navigation properties are
    /// intentionally excluded — they are never mutated by CRUD forms.
    /// </summary>
    public class ClinicFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv klinike je obavezan.")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Naziv mora imati između 2 i 200 znakova.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Adresa klinike je obavezna.")]
        [StringLength(300, ErrorMessage = "Adresa ne smije biti dulja od 300 znakova.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [StringLength(30, ErrorMessage = "Telefon ne smije biti dulji od 30 znakova.")]
        [RegularExpression(@"^[\+]?[\d\s\-\(\)\.]{6,30}$",
            ErrorMessage = "Unesite ispravan broj telefona (npr. 01 234 5678 ili +385 1 234 5678).")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        [StringLength(200, ErrorMessage = "Email ne smije biti dulji od 200 znakova.")]
        public string? Email { get; set; }

        /// <summary>
        /// Mapped to Clinic.RegistrationNumber — unique clinic identifier / tax ID.
        /// </summary>
        [StringLength(50, ErrorMessage = "Registracijski broj ne smije biti dulji od 50 znakova.")]
        public string? RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Maksimalni kapacitet je obavezan.")]
        [Range(1, 9999, ErrorMessage = "Maksimalni kapacitet mora biti između 1 i 9999.")]
        public int MaxCapacity { get; set; }

        /// <summary>
        /// Accepted as a formatted date string from the custom date picker (Create only).
        /// Edit forms leave this null — FoundationDate is immutable after creation.
        /// </summary>
        public string? FoundationDate { get; set; }
    }
}
