using System.ComponentModel.DataAnnotations;
using VetAmb.Models;

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Appointment Create / Edit.
    /// Intentionally excludes DeletedAt and navigation properties
    /// to prevent over-posting of internal / non-editable EF fields.
    /// </summary>
    public class AppointmentFormModel
    {
        public int Id { get; set; }

        // Accepted as a formatted string (dd.MM.yyyy HH:mm  or  MM/dd/yyyy hh:mm tt)
        // and parsed manually in the controller — bypasses culture-specific model binding edge cases.
        [Required(ErrorMessage = "Datum i vrijeme termina su obavezni.")]
        [Display(Name = "Datum i Vrijeme")]
        public string AppointmentDateTime { get; set; } = "";

        [Required(ErrorMessage = "Razlog posjete je obavezan.")]
        [StringLength(300, MinimumLength = 3,
            ErrorMessage = "Razlog mora sadržavati između 3 i 300 znakova.")]
        [Display(Name = "Razlog")]
        public string Reason { get; set; } = "";

        [Required(ErrorMessage = "Status termina je obavezan.")]
        [Display(Name = "Status")]
        public AppointmentStatus Status { get; set; }

        [StringLength(1000, ErrorMessage = "Napomene ne smiju biti dulje od 1000 znakova.")]
        [Display(Name = "Napomene")]
        public string? Notes { get; set; }

        [StringLength(500, ErrorMessage = "Razlog odgode ne smije biti dulji od 500 znakova.")]
        [Display(Name = "Razlog Odgode")]
        public string? RescheduleReason { get; set; }

        [Required(ErrorMessage = "Odabir pacijenta je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite pacijenta s popisa.")]
        [Display(Name = "Pacijent")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Odabir veterinara je obavezan.")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite veterinara s popisa.")]
        [Display(Name = "Veterinar")]
        public int VetId { get; set; }

        [Display(Name = "Usluge")]
        public List<int> ServiceIds { get; set; } = new();
    }
}
