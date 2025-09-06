using Microsoft.AspNetCore.Mvc;

namespace BOCS.Controllers
{
    public class MyCoursesController : Controller
    {
        public IActionResult InProgressIndex()
        {
            return View();
        }
        public IActionResult CompletedIndex()
        {
            return View();
        }
    }
}
