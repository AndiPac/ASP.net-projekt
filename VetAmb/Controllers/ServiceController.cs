using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceController(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public IActionResult Index()
        {
            var services = _serviceRepository.GetAll();
            return View(services);
        }

        public IActionResult Details(int id)
        {
            var service = _serviceRepository.GetById(id);
            if (service == null)
                return NotFound();
            return View(service);
        }
    }
}
