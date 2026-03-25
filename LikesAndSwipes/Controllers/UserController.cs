using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataRepository _dataRepository;

        public UserController(
            UserManager<User> userManager,
            DataRepository dataRepository)
        {
            _userManager = userManager;
            _dataRepository = dataRepository;
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserPage(string? id)
        {
            var userId = string.IsNullOrWhiteSpace(id)
                ? _userManager.GetUserId(User)
                : id;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var photos = await _dataRepository.GetUserPhotos(userId);

            var viewModel = new UserProfilePhotosViewModel
            {
                UserId = userId,
                Photos = photos
            };

            return View(viewModel);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserPageWithId(string id)
        {
            var photos = await _dataRepository.GetUserPhotos(id);

            var viewModel = new UserProfilePhotosViewModel
            {
                UserId = id,
                Photos = photos
            };

            return View("User", viewModel);
        }
    }
}
