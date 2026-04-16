using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IPatientRepository _patientRepo;

        public IndexModel(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public List<Patient> Patients { get; set; } = new();

        public void OnGet()
        {
            Patients = _patientRepo.GetAll();
        }
    }
}
