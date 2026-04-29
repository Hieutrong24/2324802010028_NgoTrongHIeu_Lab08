using ASC.Utilities;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ASC.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ProviderDisplayName { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Page(
                "./ExternalLogin",
                pageHandler: "Callback",
                values: new { returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= "/ServiceRequests/Dashboard/Dashboard";

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    var signedInUser = await _userManager.FindByEmailAsync(email);
                    await SetCurrentUserSessionAsync(signedInUser);
                }

                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.",
                    info.Principal.Identity?.Name,
                    info.LoginProvider);

                return LocalRedirect(GetDashboardReturnUrl(returnUrl));
            }

            if (signInResult.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;

            var externalEmail = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(externalEmail))
            {
                ModelState.AddModelError(string.Empty, "Không lấy được email từ tài khoản Google.");
                return Page();
            }

            Input.Email = externalEmail;

            var existingUser = await _userManager.FindByEmailAsync(externalEmail);

            if (existingUser != null &&
                (await _userManager.IsInRoleAsync(existingUser, "Admin") ||
                 await _userManager.IsInRoleAsync(existingUser, "Engineer")))
            {
                ModelState.AddModelError(string.Empty, $"Email '{externalEmail}' is already taken.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
        {
            returnUrl ??= "/ServiceRequests/Dashboard/Dashboard";

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            ProviderDisplayName = info.ProviderDisplayName ?? info.LoginProvider;
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingUser != null)
            {
                if (await _userManager.IsInRoleAsync(existingUser, "Admin") ||
                    await _userManager.IsInRoleAsync(existingUser, "Engineer"))
                {
                    ModelState.AddModelError(string.Empty, $"Email '{Input.Email}' is already taken.");
                    return Page();
                }

                var addLoginToExistingUserResult = await _userManager.AddLoginAsync(existingUser, info);

                if (!addLoginToExistingUserResult.Succeeded)
                {
                    foreach (var error in addLoginToExistingUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return Page();
                }

                await EnsureRoleExistsAsync("User");

                if (!await _userManager.IsInRoleAsync(existingUser, "User"))
                {
                    await _userManager.AddToRoleAsync(existingUser, "User");
                }

                await _signInManager.SignInAsync(existingUser, isPersistent: false, info.LoginProvider);
                await SetCurrentUserSessionAsync(existingUser);

                return LocalRedirect(GetDashboardReturnUrl(returnUrl));
            }

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);

            var emailStore = GetEmailStore();
            await emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            user.EmailConfirmed = true;

            var createUserResult = await _userManager.CreateAsync(user);

            if (createUserResult.Succeeded)
            {
                await EnsureRoleExistsAsync("User");
                await _userManager.AddToRoleAsync(user, "User");

                var addLoginResult = await _userManager.AddLoginAsync(user, info);

                if (addLoginResult.Succeeded)
                {
                    _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                    await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                    await SetCurrentUserSessionAsync(user);

                    return LocalRedirect(GetDashboardReturnUrl(returnUrl));
                }

                foreach (var error in addLoginResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            foreach (var error in createUserResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Cannot create an instance of '{nameof(IdentityUser)}'.");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private async Task SetCurrentUserSessionAsync(IdentityUser? user)
        {
            if (user == null)
            {
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);

            var currentUser = new CurrentUser
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };
            HttpContext.Session.SetSession("CurrentUser", currentUser);
        }

        private string GetDashboardReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
            {
                return "/ServiceRequests/Dashboard/Dashboard";
            }

            return returnUrl;
        }
    }
}