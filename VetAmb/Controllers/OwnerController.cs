using Microsoft.AspNetCore.Mvc;
using VetAmb.Repositories;

namespace VetAmb.Controllers
{
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnerController(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public IActionResult Index()
        {
            var owners = _ownerRepository.GetAll();
            return View(owners);
        }

        public IActionResult Details(int id)
        {
            var owner = _ownerRepository.GetById(id);
            if (owner == null)
                return NotFound();
            return View(owner);
        }
    }
}
