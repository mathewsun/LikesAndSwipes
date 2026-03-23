using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class UserController : Controller
    {
        [HttpGet("user")]
        public IActionResult User(string id)
        {
            return View();
        }

        [HttpGet("user/{id}")]
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}
