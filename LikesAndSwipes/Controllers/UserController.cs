using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private DataRepository _dataRepository;

        public UserController(
            UserManager<User> userManager,
            DataRepository dataRepository)
        {

            _dataRepository = dataRepository;
        }

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
