// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using LikesAndSwipes.Models;
using LikesAndSwipes.Repositories;
using LikesAndSwipes.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using LikesAndSwipes.Extensions;

namespace LikesAndSwipes.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IMinioStorageService _minioStorageService;
        private DataRepository _dataRepository;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IMinioStorageService minioStorageService,
            DataRepository dataRepository)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _minioStorageService = minioStorageService;
            _dataRepository = dataRepository;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        [BindProperty]
        public List<IFormFile> Photos { get; set; } = new();

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            /// <summary>
            ///     User Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///     User Sex
            /// </summary>
            [Required]
            public bool? Sex { get; set; }

            /// <summary>
            ///     Romantic preference for men
            /// </summary>
            public bool RomanticMen { get; set; }

            /// <summary>
            ///     Romantic preference for women
            /// </summary>
            public bool RomanticWomen { get; set; }

            /// <summary>
            ///     Friendship preference for men
            /// </summary>
            public bool FriendshipMen { get; set; }

            /// <summary>
            ///     Friendship preference for women
            /// </summary>
            public bool FriendshipWomen { get; set; }

            /// <summary>
            ///     User Age
            /// </summary>
            public int Age { get; set; }

            /// <summary>
            ///     Selected interests during registration
            /// </summary>
            public List<string> SelectedInterests { get; set; } = new();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    string newUserName = StringExtensions.ConvertToLatin(Input.Name);

                    //checking username not exist
                    while (await _dataRepository.CheckUserNameExist(newUserName))
                    {
                        Random random = new Random();
                        int randomNumber = random.Next(100, 1001);
                        newUserName = newUserName + randomNumber.ToString();
                    }

                    User newUser = new User()
                    {
                        Id = user.Id,
                        FirstName = Input.Name,
                        UserName = newUserName,
                        Sex = Input.Sex ?? false,
                        RomanticMen = Input.RomanticMen,
                        RomanticWomen = Input.RomanticWomen,
                        FriendshipMen = Input.FriendshipMen,
                        FriendshipWomen = Input.FriendshipWomen,
                        Age = Input.Age
                    };

                    await _dataRepository.SaveUserRegistrationData(newUser);
                    await _dataRepository.SaveUserInterests(user.Id, Input.SelectedInterests);

                    try
                    {
                        var uploadedPhotos = await UploadRegistrationPhotosAsync(user.Id);
                        await _dataRepository.SaveUserPhotos(user.Id, uploadedPhotos);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to upload registration photos for user {UserId}", user.Id);
                        //await _userManager.DeleteAsync(user);
                        ModelState.AddModelError(string.Empty, "Не удалось сохранить фотографии.");
                        return Page();
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }


        private async Task<List<UserPhoto>> UploadRegistrationPhotosAsync(string userId)
        {
            var validPhotos = Photos
                .Where(photo => photo is not null && photo.Length > 0)
                .Take(6)
                .ToList();

            //if (validPhotos.Count < 2)
            //{
            //    throw new InvalidOperationException("At least two photos are required for registration.");
            //}

            var uploadedPhotos = new List<UserPhoto>();
            var uploadedObjectNames = new List<string>();

            try
            {
                for (var index = 0; index < validPhotos.Count; index++)
                {
                    var photo = validPhotos[index];
                    var extension = Path.GetExtension(photo.FileName);
                    var objectName = $"users/{userId}/registration/{index + 1}-{Guid.NewGuid():N}{extension}";
                    var uploadResult = await _minioStorageService.UploadAsync(photo, objectName);

                    uploadedObjectNames.Add(uploadResult.ObjectName);
                    uploadedPhotos.Add(new UserPhoto
                    {
                        UserId = userId,
                        ObjectName = uploadResult.ObjectName,
                        ContentType = photo.ContentType ?? string.Empty,
                        SortOrder = index
                    });
                }

                return uploadedPhotos;
            }
            catch
            {
                foreach (var objectName in uploadedObjectNames)
                {
                    await _minioStorageService.DeleteAsync(objectName);
                }

                throw;
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
