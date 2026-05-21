using System.ComponentModel.DataAnnotations;

namespace VetAmb.Models
{
    /// <summary>
    /// Enum for veterinary specializations
    /// </summary>
    public enum VeterinarySpecialization
    {
        [Display(Name = "Opća praksa")]
        GeneralPractice,
        [Display(Name = "Kirurgija")]
        Surgery,
        [Display(Name = "Dentalna medicina")]
        Dentistry,
        [Display(Name = "Kardiologija")]
        Cardiology,
        [Display(Name = "Dermatologija")]
        Dermatology,
        [Display(Name = "Ortopedija")]
        Orthopedics,
        [Display(Name = "Interna medicina")]
        InternalMedicine
    }

    /// <summary>
    /// Enum for appointment status
    /// </summary>
    public enum AppointmentStatus
    {
        [Display(Name = "Zakazan")]
        Scheduled,
        [Display(Name = "U tijeku")]
        InProgress,
        [Display(Name = "Završen")]
        Completed,
        [Display(Name = "Otkazan")]
        Cancelled,
        [Display(Name = "Nije došao")]
        NoShow,
        [Display(Name = "Premješten")]
        Rescheduled
    }

    /// <summary>
    /// Enum for animal species
    /// </summary>
    public enum AnimalSpecies
    {
        [Display(Name = "Pas")]
        Dog,
        [Display(Name = "Mačka")]
        Cat,
        [Display(Name = "Ptica")]
        Bird,
        [Display(Name = "Kunić")]
        Rabbit,
        [Display(Name = "Hrčak")]
        Hamster,
        [Display(Name = "Zamorac")]
        Guinea_Pig,
        [Display(Name = "Tvor")]
        Ferret,
        [Display(Name = "Gmaz")]
        Reptile
    }
}
