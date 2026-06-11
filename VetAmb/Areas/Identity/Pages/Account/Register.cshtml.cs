using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
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
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "Email adresa je obavezna.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [StringLength(100, ErrorMessage = "Lozinka mora imati najmanje {2} i najviše {1} znakova.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Potvrda lozinke")]
            [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")]
            public string ConfirmPassword { get; set; } = string.Empty;

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

            var existingOib = await _userManager.Users
                .AnyAsync(u => u.OIB == Input.OIB);
            if (existingOib)
            {
                ModelState.AddModelError(nameof(Input.OIB), "Korisnik s ovim OIB-om već postoji.");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            var existingJmbg = await _userManager.Users
                .AnyAsync(u => u.JMBG == Input.JMBG);
            if (existingJmbg)
            {
                ModelState.AddModelError(nameof(Input.JMBG), "Korisnik s ovim JMBG-om već postoji.");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                OIB = Input.OIB,
                JMBG = Input.JMBG
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

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
    }
}
