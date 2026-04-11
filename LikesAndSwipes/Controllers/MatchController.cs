using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class MatchController : Controller
    {
        private DataRepository _dataRepository;

        public MatchController(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public IActionResult Index()
        {


            return View();
        }
    }
}
