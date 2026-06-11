using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetAmb.Models;

#nullable enable

namespace VetAmb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginConfirmationModel : PageModel
    {
        private const string GenericDuplicateErrorMessage = "Niste unijeli ispravan oib";

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalLoginConfirmationModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? Email { get; set; }
        public string? LoginProvider { get; set; }
        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "OIB je obavezan.")]
            [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
            [RegularExpression("^[0-9]{11}$", ErrorMessage = "OIB mora sadržavati točno 11 znamenki.")]
            [Display(Name = "OIB")]
            public string OIB { get; set; } = string.Empty;

            [Required(ErrorMessage = "JMBG je obavezan.")]
            [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG mora sadržavati točno 13 znamenki.")]
            [RegularExpression("^[0-9]{13}$", ErrorMessage = "JMBG mora sadržavati točno 13 znamenki.")]
            [Display(Name = "JMBG")]
            public string JMBG { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Login");
            }

            Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl ?? Url.Content("~/");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Vanjska prijava je istekla. Pokušajte se ponovo prijaviti.");
                return Page();
            }

            Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            LoginProvider = info.LoginProvider;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Duplicate OIB / JMBG checks before any CreateAsync call.
            if (await _userManager.Users.AnyAsync(u => u.OIB == Input.OIB))
            {
                ModelState.AddModelError(string.Empty, GenericDuplicateErrorMessage);
                return Page();
            }

            if (await _userManager.Users.AnyAsync(u => u.JMBG == Input.JMBG))
            {
                ModelState.AddModelError(string.Empty, GenericDuplicateErrorMessage);
                return Page();
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(string.Empty, "Google račun nije vratio email adresu.");
                return Page();
            }

            var user = new AppUser
            {
                UserName = Email,
                Email = Email,
                OIB = Input.OIB,
                JMBG = Input.JMBG,
                EmailConfirmed = true
            };

            IdentityResult createResult;
            try
            {
                createResult = await _userManager.CreateAsync(user);
            }
            catch (DbUpdateException ex) when (IsUniqueOibOrJmbgViolation(ex))
            {
                // Covers race-condition conflicts against DB unique indexes.
                ModelState.AddModelError(string.Empty, GenericDuplicateErrorMessage);
                return Page();
            }

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _userManager.AddToRoleAsync(user, "User");

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                foreach (var error in addLoginResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        private static bool IsUniqueOibOrJmbgViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;
            return message.Contains("IX_AspNetUsers_OIB", StringComparison.OrdinalIgnoreCase)
                || message.Contains("IX_AspNetUsers_JMBG", StringComparison.OrdinalIgnoreCase);
        }
    }
}
