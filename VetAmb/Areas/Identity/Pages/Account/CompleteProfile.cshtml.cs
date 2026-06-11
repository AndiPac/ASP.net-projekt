using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VetAmb.Models;

#nullable enable

namespace VetAmb.Areas.Identity.Pages.Account
{
    [Authorize]
    public class CompleteProfileModel : PageModel
    {
        private const string PlaceholderOib = "00000000000";
        private const string PlaceholderJmbg = "0000000000000";

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CompleteProfileModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (!NeedsProfileCompletion(user))
            {
                return LocalRedirect(returnUrl);
            }

            Input.OIB = user.OIB == PlaceholderOib ? string.Empty : user.OIB;
            Input.JMBG = user.JMBG == PlaceholderJmbg ? string.Empty : user.JMBG;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("./Login");
            }

            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (Input.OIB == PlaceholderOib)
            {
                ModelState.AddModelError("Input.OIB", "Unesite stvarni OIB.");
            }

            if (Input.JMBG == PlaceholderJmbg)
            {
                ModelState.AddModelError("Input.JMBG", "Unesite stvarni JMBG.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Duplicate checks — exclude the current user's own values.
            if (await _userManager.Users.AnyAsync(u => u.OIB == Input.OIB && u.Id != user.Id))
            {
                ModelState.AddModelError("Input.OIB", "Korisnik s ovim OIB-om već postoji.");
                return Page();
            }

            if (await _userManager.Users.AnyAsync(u => u.JMBG == Input.JMBG && u.Id != user.Id))
            {
                ModelState.AddModelError("Input.JMBG", "Korisnik s ovim JMBG-om već postoji.");
                return Page();
            }

            user.OIB = Input.OIB;
            user.JMBG = Input.JMBG;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            return LocalRedirect(returnUrl);
        }

        private static bool NeedsProfileCompletion(AppUser user)
        {
            if (string.IsNullOrWhiteSpace(user.OIB) || string.IsNullOrWhiteSpace(user.JMBG))
            {
                return true;
            }

            return user.OIB == PlaceholderOib || user.JMBG == PlaceholderJmbg;
        }
    }
}
