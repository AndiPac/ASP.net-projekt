using System.ComponentModel.DataAnnotations;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Owner Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt, RegistrationDate, and navigation properties are intentionally excluded.
    /// RegistrationDate is set automatically on Create (DateTime.UtcNow) and kept on Edit.
    /// ClinicId is required (FK) and is bound via AJAX autocomplete.
    /// </summary>
    public class OwnerFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime vlasnika je obavezno.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Prezime vlasnika je obavezno.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "E-mail adresa je obavezna.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu e-mail adresu.")]
        [StringLength(200, ErrorMessage = "E-mail ne smije biti dulji od 200 znakova.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [StringLength(30, ErrorMessage = "Broj telefona ne smije biti dulji od 30 znakova.")]
        [RegularExpression(@"^[\+]?[\d\s\-\(\)\.]{6,30}$",
            ErrorMessage = "Unesite ispravan broj telefona (npr. 091 234 5678 ili +385 91 234 5678).")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Adresa vlasnika je obavezna.")]
        [StringLength(300, ErrorMessage = "Adresa ne smije biti dulja od 300 znakova.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Broj osobne iskaznice je obavezan.")]
        [StringLength(50, ErrorMessage = "Broj osobne iskaznice ne smije biti dulji od 50 znakova.")]
        public string? IdNumber { get; set; }

        /// <summary>
        /// FK to Clinic — required. Bound via AJAX autocomplete; validated server- and client-side.
        /// </summary>
        [Required(ErrorMessage = "Odabir klinike je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite kliniku s popisa.")]
        public int ClinicId { get; set; }
    }
}
