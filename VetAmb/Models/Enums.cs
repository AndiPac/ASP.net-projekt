namespace VetAmb.Models
{
    /// <summary>
    /// Enum for veterinary specializations
    /// </summary>
    public enum VeterinarySpecialization
    {
        GeneralPractice,
        Surgery,
        Dentistry,
        Cardiology,
        Dermatology,
        Orthopedics,
        InternalMedicine
    }

    /// <summary>
    /// Enum for appointment status
    /// </summary>
    public enum AppointmentStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled,
        NoShow,
        Rescheduled
    }

    /// <summary>
    /// Enum for animal species
    /// </summary>
    public enum AnimalSpecies
    {
        Dog,
        Cat,
        Bird,
        Rabbit,
        Hamster,
        Guinea_Pig,
        Ferret,
        Reptile
    }
}
