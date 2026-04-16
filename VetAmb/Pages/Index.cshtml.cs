using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPatientRepository _patientRepo;
        private readonly IVetRepository _vetRepo;
        private readonly IClinicRepository _clinicRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly IAppointmentRepository _appointmentRepo;

        public IndexModel(
            IPatientRepository patientRepo,
            IVetRepository vetRepo,
            IClinicRepository clinicRepo,
            IOwnerRepository ownerRepo,
            IAppointmentRepository appointmentRepo)
        {
            _patientRepo = patientRepo;
            _vetRepo = vetRepo;
            _clinicRepo = clinicRepo;
            _ownerRepo = ownerRepo;
            _appointmentRepo = appointmentRepo;
        }

        public int TotalPatients { get; set; }
        public int TotalVets { get; set; }
        public int TotalClinics { get; set; }
        public int TotalOwners { get; set; }
        public int TotalAppointments { get; set; }
        public List<Appointment> RecentAppointments { get; set; } = new();

        public void OnGet()
        {
            TotalPatients = _patientRepo.GetAll().Count;
            TotalVets = _vetRepo.GetAll().Count;
            TotalClinics = _clinicRepo.GetAll().Count;
            TotalOwners = _ownerRepo.GetAll().Count;
            TotalAppointments = _appointmentRepo.GetAll().Count;
            RecentAppointments = _appointmentRepo.GetAll()
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(4)
                .ToList();
        }
    }
}
