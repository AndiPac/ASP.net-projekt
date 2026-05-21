using System.ComponentModel.DataAnnotations;
using VetAmb.Models;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Patient Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt and navigation properties are intentionally excluded.
    /// </summary>
    public class PatientFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime pacijenta je obavezno.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vrsta životinje je obavezna.")]
        public AnimalSpecies Species { get; set; }

        [StringLength(100, ErrorMessage = "Pasmina ne smije biti dulja od 100 znakova.")]
        public string? Breed { get; set; }

        /// <summary>
        /// Accepted as a string so the custom date picker's "dd.MM.yyyy" or
        /// "MM/dd/yyyy" output maps cleanly without culture-dependent binding issues.
        /// </summary>
        [Required(ErrorMessage = "Datum rođenja je obavezan.")]
        public string? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Težina je obavezna.")]
        [Range(0.01, 999.99,
            ErrorMessage = "Težina mora biti između 0.01 kg i 999.99 kg.")]
        public decimal Weight { get; set; }

        [StringLength(50, ErrorMessage = "Broj mikročipa ne smije biti dulji od 50 znakova.")]
        public string? MicrochipId { get; set; }

        [StringLength(50, ErrorMessage = "Boja ne smije biti dulja od 50 znakova.")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Odabir vlasnika je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite vlasnika s popisa.")]
        public int OwnerId { get; set; }
    }
}
