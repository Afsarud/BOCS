using Microsoft.AspNetCore.Mvc;

namespace BOCS.Controllers
{
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
