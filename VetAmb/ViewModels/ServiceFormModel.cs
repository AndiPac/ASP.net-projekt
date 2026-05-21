using System.ComponentModel.DataAnnotations;

#nullable enable

namespace VetAmb.ViewModels
{
    /// <summary>
    /// Form ViewModel for Service Create/Edit.
    /// Prevents over-posting: only fields explicitly listed here are bound.
    /// DeletedAt and AppointmentServices navigation property are intentionally excluded.
    /// </summary>
    public class ServiceFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv usluge je obavezan.")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Naziv mora imati između 2 i 200 znakova.")]
        public string? Name { get; set; }

        [StringLength(1000, ErrorMessage = "Opis ne smije biti dulji od 1000 znakova.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna.")]
        [Range(0.01, 99999.99, ErrorMessage = "Cijena mora biti između 0.01 € i 99999.99 €.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Procijenjeno trajanje je obavezno.")]
        [Range(1, 480, ErrorMessage = "Trajanje mora biti između 1 i 480 minuta.")]
        public int EstimatedDurationMinutes { get; set; }
    }
}
