using System.ComponentModel.DataAnnotations;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for MedicalRecord Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt and Patient navigation property are intentionally excluded.
    ///
    /// DateRecorded is a string so it safely binds the custom date-picker
    /// string payload (format: dd.MM.yyyy HH:mm or MM/dd/yyyy hh:mm tt).
    /// The controller's ParseDate helper converts it to DateTime before saving.
    ///
    /// PatientId is required (FK) and is bound via AJAX autocomplete.
    /// </summary>
    public class MedicalRecordFormModel
    {
        public int Id { get; set; }

        /// <summary>
        /// String payload from _CustomDateTimePicker.
        /// Accepted formats: "dd.MM.yyyy HH:mm", "dd.MM.yyyy", "MM/dd/yyyy hh:mm tt", "MM/dd/yyyy".
        /// </summary>
        [Required(ErrorMessage = "Datum pregleda je obavezan.")]
        public string? DateRecorded { get; set; }

        [Required(ErrorMessage = "Dijagnoza je obavezna.")]
        [StringLength(500, MinimumLength = 2,
            ErrorMessage = "Dijagnoza mora imati između 2 i 500 znakova.")]
        public string? Diagnosis { get; set; }

        [Required(ErrorMessage = "Terapija je obavezna.")]
        [StringLength(1000, ErrorMessage = "Terapija ne smije biti dulja od 1000 znakova.")]
        public string? Treatment { get; set; }

        [StringLength(2000, ErrorMessage = "Napomene ne smiju biti dulje od 2000 znakova.")]
        public string? Notes { get; set; }

        /// <summary>
        /// FK to Patient — required. Bound via AJAX autocomplete → /medicalrecords/search-patients.
        /// </summary>
        [Required(ErrorMessage = "Odabir pacijenta je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite pacijenta s popisa.")]
        public int PatientId { get; set; }
    }
}
