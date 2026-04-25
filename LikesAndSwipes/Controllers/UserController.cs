using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using LikesAndSwipes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LikesAndSwipes.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DataRepository _dataRepository;
        private readonly IMinioStorageService _minioStorageService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<User> userManager,
            DataRepository dataRepository,
            IMinioStorageService minioStorageService,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _dataRepository = dataRepository;
            _minioStorageService = minioStorageService;
            _logger = logger;
        }

        /// <summary>
        /// My user profile page
        /// </summary>
        /// <returns></returns>
        [HttpGet("user")]
        public async Task<IActionResult> GetUserPage()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var photos = await _dataRepository.GetUserPhotos(userId);
            var currentUser = await _userManager.GetUserAsync(User);

            var viewModel = new UserProfilePhotosViewModel
            {
                BirthDay = currentUser?.BirthDay,
                RomanticMen = currentUser?.RomanticMen ?? false,
                RomanticWomen = currentUser?.RomanticWomen ?? false,
                FriendshipMen = currentUser?.FriendshipMen ?? false,
                FriendshipWomen = currentUser?.FriendshipWomen ?? false,
                Photos = photos
            };

            return View(viewModel);
        }

        /// <summary>
        /// Watch user profile page by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserPageWithUserName(string username)
        {
            var photos = await _dataRepository.GetUserPhotosByUserName(username);

            var viewModel = new UserProfilePhotosViewModel
            {
                UserName = username,
                Photos = photos
            };

            return View(viewModel);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("user/photo")]
        public async Task<IActionResult> AddPhoto(IFormFile? photo, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            if (photo is null || photo.Length == 0)
            {
                TempData["PhotoUploadError"] = "Выберите фотографию перед загрузкой.";
                return RedirectToAction(nameof(GetUserPage));
            }

            var extension = Path.GetExtension(photo.FileName);
            var objectName = $"users/{userId}/profile/{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}{extension}";

            try
            {
                var uploadResult = await _minioStorageService.UploadAsync(photo, objectName, cancellationToken);
                var existingPhotos = await _dataRepository.GetUserPhotos(userId);
                var nextSortOrder = existingPhotos.Count == 0
                    ? 0
                    : existingPhotos.Max(x => x.SortOrder) + 1;

                await _dataRepository.SaveUserPhotos(userId, new[]
                {
                    new UserPhoto
                    {
                        UserId = userId,
                        ObjectName = uploadResult.ObjectName,
                        ContentType = photo.ContentType ?? string.Empty,
                        SortOrder = nextSortOrder
                    }
                });

                TempData["PhotoUploadSuccess"] = "Фотография добавлена.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add profile photo for user {UserId}", userId);
                TempData["PhotoUploadError"] = "Не удалось загрузить фотографию. Попробуйте позже.";
            }

            return RedirectToAction(nameof(GetUserPage));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("user/photo/{photoId:int}/delete")]
        public async Task<IActionResult> DeletePhoto(int photoId, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var photo = await _dataRepository.GetUserPhoto(userId, photoId);
            if (photo is null)
            {
                TempData["PhotoUploadError"] = "Фотография не найдена или уже удалена.";
                return RedirectToAction(nameof(GetUserPage));
            }

            try
            {
                await _minioStorageService.DeleteAsync(photo.ObjectName, cancellationToken);
                await _dataRepository.DeleteUserPhoto(photo);
                TempData["PhotoUploadSuccess"] = "Фотография удалена.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete profile photo {PhotoId} for user {UserId}", photoId, userId);
                TempData["PhotoUploadError"] = "Не удалось удалить фотографию. Попробуйте позже.";
            }

            return RedirectToAction(nameof(GetUserPage));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("user/birthday")]
        public async Task<IActionResult> UpdateBirthDay(DateTime birthDay)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            if (birthDay == default || birthDay.Date > DateTime.UtcNow.Date || birthDay.Year < 1900)
            {
                TempData["BirthDayError"] = "Укажите корректную дату рождения.";
                return RedirectToAction(nameof(GetUserPage));
            }

            user.BirthDay = DateTime.SpecifyKind(birthDay.Date, DateTimeKind.Utc);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["BirthDaySuccess"] = "Дата рождения обновлена.";
            }
            else
            {
                TempData["BirthDayError"] = "Не удалось обновить дату рождения. Попробуйте позже.";
            }

            return RedirectToAction(nameof(GetUserPage));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost("user/preferences")]
        public async Task<IActionResult> UpdatePreferences(bool romanticMen, bool romanticWomen, bool friendshipMen, bool friendshipWomen)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Challenge();
            }

            if (!romanticMen && !romanticWomen && !friendshipMen && !friendshipWomen)
            {
                TempData["PreferenceError"] = "Выберите хотя бы одно предпочтение.";
                return RedirectToAction(nameof(GetUserPage));
            }

            user.RomanticMen = romanticMen;
            user.RomanticWomen = romanticWomen;
            user.FriendshipMen = friendshipMen;
            user.FriendshipWomen = friendshipWomen;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["PreferenceSuccess"] = "Предпочтения обновлены.";
            }
            else
            {
                TempData["PreferenceError"] = "Не удалось обновить предпочтения. Попробуйте позже.";
            }

            return RedirectToAction(nameof(GetUserPage));
        }
    }
}
