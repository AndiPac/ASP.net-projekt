using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VetAmb.Models;

#nullable enable

namespace VetAmb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private const string PlaceholderOib = "00000000000";
        private const string PlaceholderJmbg = "0000000000000";

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "Email adresa je obavezna.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Zapamti me")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                Input.Email,
                Input.Password,
                Input.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Neispravna prijava.");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostExternalLogin(string provider, string? returnUrl = null)
        {
            var externalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
            if (!externalLogins.Any(s => string.Equals(s.Name, provider, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError(string.Empty, "Prijava putem Googlea trenutno nije dostupna.");
                ReturnUrl = returnUrl;
                ExternalLogins = externalLogins.ToList();
                return Page();
            }

            var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetExternalLoginCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Vanjska prijava nije uspjela: {remoteError}");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("./Login");
            }

            // Case 1: user already has this external login linked — sign them in directly.
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                var linkedUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                if (linkedUser != null)
                {
                    if (!await _userManager.IsInRoleAsync(linkedUser, "User"))
                    {
                        await _userManager.AddToRoleAsync(linkedUser, "User");
                    }

                    if (NeedsProfileCompletion(linkedUser))
                    {
                        return RedirectToPage("./CompleteProfile", new { returnUrl });
                    }
                }

                return LocalRedirect(returnUrl);
            }

            // Case 2: a local account already exists with the same email — link and sign in.
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrWhiteSpace(email))
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    var logins = await _userManager.GetLoginsAsync(existingUser);
                    if (!logins.Any(l => l.LoginProvider == info.LoginProvider && l.ProviderKey == info.ProviderKey))
                    {
                        await _userManager.AddLoginAsync(existingUser, info);
                    }

                    if (!await _userManager.IsInRoleAsync(existingUser, "User"))
                    {
                        await _userManager.AddToRoleAsync(existingUser, "User");
                    }

                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            // Case 3: brand-new user — redirect to confirmation page to collect OIB/JMBG
            //         before any CreateAsync call, avoiding unique-constraint violations.
            return RedirectToPage("./ExternalLoginConfirmation", new { returnUrl });
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
