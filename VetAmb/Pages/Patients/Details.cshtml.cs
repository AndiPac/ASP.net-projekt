using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetAmb.Models;
using VetAmb.Repositories;

namespace VetAmb.Pages.Patients
{
    public class DetailsModel : PageModel
    {
        private readonly IPatientRepository _patientRepo;

        public DetailsModel(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public Patient? Patient { get; set; }

        public IActionResult OnGet(int id)
        {
            Patient = _patientRepo.GetById(id);
            if (Patient == null)
                return NotFound();
            return Page();
        }
    }
}
