using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace LikesAndSwipes.Controllers
{
    public class HomeController : Controller
    {
        private DataRepository _dataRepository;

        public HomeController(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _dataRepository.GetAllInterests();

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                User user = new User();

                user.FirstName = name;
                user.Id = userId;

                await _dataRepository.SaveUserFirstName(user);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
