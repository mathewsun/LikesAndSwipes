using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LikesAndSwipes.Controllers
{
    public class MatchController : Controller
    {
        private DataRepository _dataRepository;

        public MatchController(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                List<User> romanticResult = await _dataRepository.GetUserRomanticRecomendations(userId);

                return View(romanticResult);
            }
            else 
                return View();
        }
    }
}
