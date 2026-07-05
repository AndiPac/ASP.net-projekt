using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VetAmb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class AccessDeniedModel : PageModel
    {
        public string BackUrl { get; private set; } = "/";

        public void OnGet()
        {
            var returnUrl = Request.Query["ReturnUrl"].ToString();
            var fallbackFromReturnUrl = GetFallbackFromReturnUrl(returnUrl);
            if (!string.IsNullOrWhiteSpace(fallbackFromReturnUrl))
            {
                BackUrl = fallbackFromReturnUrl;
                return;
            }

            var refererHeader = Request.Headers.Referer.ToString();
            if (Uri.TryCreate(refererHeader, UriKind.Absolute, out var referer)
                && string.Equals(referer.Host, Request.Host.Host, StringComparison.OrdinalIgnoreCase))
            {
                var candidatePath = referer.PathAndQuery;
                if (!string.IsNullOrWhiteSpace(candidatePath)
                    && !candidatePath.StartsWith("/Identity/Account/AccessDenied", StringComparison.OrdinalIgnoreCase)
                    && !candidatePath.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase)
                    && !candidatePath.StartsWith("/signin-google", StringComparison.OrdinalIgnoreCase))
                {
                    BackUrl = candidatePath;
                }
            }
        }

        private static string? GetFallbackFromReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !returnUrl.StartsWith('/'))
            {
                return null;
            }

            var trimmed = returnUrl.Split('?', '#')[0].TrimEnd('/');
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return null;
            }

            if (trimmed.EndsWith("/create", StringComparison.OrdinalIgnoreCase))
            {
                var basePath = trimmed[..^"/create".Length];
                return string.IsNullOrWhiteSpace(basePath) ? "/" : basePath;
            }

            if (trimmed.EndsWith("/edit", StringComparison.OrdinalIgnoreCase))
            {
                var lastSlash = trimmed.LastIndexOf('/');
                if (lastSlash > 0)
                {
                    var previousSlash = trimmed.LastIndexOf('/', lastSlash - 1);
                    if (previousSlash > 0)
                    {
                        return trimmed[..previousSlash];
                    }
                }
            }

            return null;
        }
    }
}