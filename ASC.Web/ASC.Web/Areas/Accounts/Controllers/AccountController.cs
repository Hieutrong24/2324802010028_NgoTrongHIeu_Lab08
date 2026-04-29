using ASC.Web.Areas.Accounts.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace ASC.Web.Areas.Accounts.Controllers
{
    [Area("Accounts")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ServiceEngineers()
        {
            var model = await GetServiceEngineerViewModelAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceEngineers(ServiceEngineerViewModel model)
        {
            var isUpdate = !string.IsNullOrWhiteSpace(model.ServiceEngineer.Id);

            if (!isUpdate && string.IsNullOrWhiteSpace(model.ServiceEngineer.Password))
            {
                ModelState.AddModelError("ServiceEngineer.Password", "Password is required");
            }

            if (!ModelState.IsValid)
            {
                var reloadModel = await GetServiceEngineerViewModelAsync();
                reloadModel.ServiceEngineer = model.ServiceEngineer;
                return View(reloadModel);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.ServiceEngineer.Email);

            if (!isUpdate && existingUser != null)
            {
                ModelState.AddModelError("ServiceEngineer.Email", "Email is already taken");

                var reloadModel = await GetServiceEngineerViewModelAsync();
                reloadModel.ServiceEngineer = model.ServiceEngineer;
                return View(reloadModel);
            }

            if (isUpdate)
            {
                var userToUpdate = await _userManager.FindByIdAsync(model.ServiceEngineer.Id!);

                if (userToUpdate == null)
                {
                    ModelState.AddModelError(string.Empty, "Service Engineer not found.");

                    var reloadModel = await GetServiceEngineerViewModelAsync();
                    reloadModel.ServiceEngineer = model.ServiceEngineer;
                    return View(reloadModel);
                }

                var emailOwner = await _userManager.FindByEmailAsync(model.ServiceEngineer.Email);

                if (emailOwner != null && emailOwner.Id != userToUpdate.Id)
                {
                    ModelState.AddModelError("ServiceEngineer.Email", "Email is already taken by another user.");

                    var reloadModel = await GetServiceEngineerViewModelAsync();
                    reloadModel.ServiceEngineer = model.ServiceEngineer;
                    return View(reloadModel);
                }

                userToUpdate.UserName = model.ServiceEngineer.UserName;
                userToUpdate.Email = model.ServiceEngineer.Email;
                userToUpdate.EmailConfirmed = true;

                if (model.ServiceEngineer.IsActive)
                {
                    userToUpdate.LockoutEnabled = false;
                    userToUpdate.LockoutEnd = null;
                }
                else
                {
                    userToUpdate.LockoutEnabled = true;
                    userToUpdate.LockoutEnd = DateTimeOffset.MaxValue;
                }

                var updateResult = await _userManager.UpdateAsync(userToUpdate);

                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var reloadModel = await GetServiceEngineerViewModelAsync();
                    reloadModel.ServiceEngineer = model.ServiceEngineer;
                    return View(reloadModel);
                }

                if (!string.IsNullOrWhiteSpace(model.ServiceEngineer.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(userToUpdate);
                    var resetResult = await _userManager.ResetPasswordAsync(
                        userToUpdate,
                        token,
                        model.ServiceEngineer.Password);

                    if (!resetResult.Succeeded)
                    {
                        foreach (var error in resetResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        var reloadModel = await GetServiceEngineerViewModelAsync();
                        reloadModel.ServiceEngineer = model.ServiceEngineer;
                        return View(reloadModel);
                    }
                }

                if (!await _userManager.IsInRoleAsync(userToUpdate, "Engineer"))
                {
                    await _userManager.AddToRoleAsync(userToUpdate, "Engineer");
                }

                TempData["SuccessMessage"] = "Service Engineer updated successfully.";
                await _emailSender.SendEmailAsync(
                        userToUpdate.Email!,
                        "ASC - Service Engineer Account Updated",
                        $"Your service engineer account has been updated.<br/>Email: {userToUpdate.Email}<br/>Username: {userToUpdate.UserName}<br/>Status: {(model.ServiceEngineer.IsActive ? "Active" : "Deactive")}");
                return RedirectToAction(nameof(ServiceEngineers));
            }



            var user = new IdentityUser
            {
                UserName = model.ServiceEngineer.UserName,
                Email = model.ServiceEngineer.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.ServiceEngineer.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var reloadModel = await GetServiceEngineerViewModelAsync();
                reloadModel.ServiceEngineer = model.ServiceEngineer;
                return View(reloadModel);
            }

            if (!await _roleManager.RoleExistsAsync("Engineer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Engineer"));
            }

            await _userManager.AddToRoleAsync(user, "Engineer");
            await _emailSender.SendEmailAsync(
                user.Email!,
                "ASC - Service Engineer Account Created",
                $"Your service engineer account has been created.<br/>Email: {user.Email}<br/>Username: {user.UserName}");
            if (!model.ServiceEngineer.IsActive)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTimeOffset.MaxValue;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "Service Engineer created successfully.";

            return RedirectToAction(nameof(ServiceEngineers));
        }

        private async Task<ServiceEngineerViewModel> GetServiceEngineerViewModelAsync()
        {
            var engineerUsers = await _userManager.GetUsersInRoleAsync("Engineer");

            return new ServiceEngineerViewModel
            {
                ServiceEngineers = engineerUsers.Select(user => new ServiceEngineerRegistrationViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                }).ToList()
            };
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Customers()
        {
            var customerUsers = await _userManager.GetUsersInRoleAsync("User");

            var model = new CustomerViewModel
            {
                Customers = customerUsers.Select(user => new CustomerRegistrationViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                }).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Customers(CustomerViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Customer.Id))
            {
                ModelState.AddModelError(string.Empty, "Please select a customer to update.");

                var reloadModel = await GetCustomerViewModelAsync();
                reloadModel.Customer = model.Customer;
                return View(reloadModel);
            }

            var customer = await _userManager.FindByIdAsync(model.Customer.Id);

            if (customer == null)
            {
                ModelState.AddModelError(string.Empty, "Customer not found.");

                var reloadModel = await GetCustomerViewModelAsync();
                reloadModel.Customer = model.Customer;
                return View(reloadModel);
            }

            if (model.Customer.IsActive)
            {
                customer.LockoutEnabled = false;
                customer.LockoutEnd = null;
            }
            else
            {
                customer.LockoutEnabled = true;
                customer.LockoutEnd = DateTimeOffset.MaxValue;
            }

            var updateResult = await _userManager.UpdateAsync(customer);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var reloadModel = await GetCustomerViewModelAsync();
                reloadModel.Customer = model.Customer;
                return View(reloadModel);
            }

            await _emailSender.SendEmailAsync(
                customer.Email!,
                "ASC - Customer Account Updated",
                $"Your customer account status has been updated.<br/>Email: {customer.Email}<br/>Status: {(model.Customer.IsActive ? "Active" : "Deactive")}");

            TempData["SuccessMessage"] = "Customer updated successfully.";

            return RedirectToAction(nameof(Customers));
        }

        private async Task<CustomerViewModel> GetCustomerViewModelAsync()
        {
            var customerUsers = await _userManager.GetUsersInRoleAsync("User");

            return new CustomerViewModel
            {
                Customers = customerUsers.Select(user => new CustomerRegistrationViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    IsActive = !(user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTimeOffset.UtcNow)
                }).ToList()
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var model = new ProfileModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Profile(ProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, model.UserName);

            if (!setUserNameResult.Succeeded)
            {
                foreach (var error in setUserNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);

            TempData["SuccessMessage"] = "Profile updated successfully.";

            return RedirectToAction(nameof(Profile));
        }
    }
}