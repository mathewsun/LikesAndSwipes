using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class MatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
