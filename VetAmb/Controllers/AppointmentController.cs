using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public IActionResult Index()
        {
            var appointments = _appointmentRepository.GetAll();
            return View(appointments);
        }

        public IActionResult Details(int id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
                return NotFound();
            return View(appointment);
        }
    }
}
