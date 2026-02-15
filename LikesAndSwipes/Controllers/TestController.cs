using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using System.Security.Claims;

namespace LikesAndSwipes.Controllers
{
    public class TestController : Controller
    {
        private DataRepository _dataRepository;

        public TestController(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Point()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Point(TestPointPageModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                LocationEntity location = new LocationEntity
                {
                    UserId = userId,
                    Location = new Point(new Coordinate(model.XCoordinate, model.YCoordinate))
                };

                var result = await _dataRepository.SaveUserLocation(location);
            }

            return RedirectToAction(nameof(Point));
        }
    }
}
