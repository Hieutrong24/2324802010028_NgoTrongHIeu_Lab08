using ASC.Model;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ASC.Web.Data
{
    public class IdentitySeed : IIdentitySeed
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOptions<AdminUserSettings> _adminSettings;

        public IdentitySeed(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AdminUserSettings> adminSettings)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminSettings = adminSettings;
        }

        public async Task SeedAsync()
        {
            await SeedMasterDataAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedMasterDataAsync()
        {
            if (!_context.MasterDataKeys.Any())
            {
                var serviceTypeKey = new MasterDataKey
                {
                    Name = "ServiceType",
                    CreatedBy = "System"
                };

                var vehicleTypeKey = new MasterDataKey
                {
                    Name = "VehicleType",
                    CreatedBy = "System"
                };

                await _context.MasterDataKeys.AddRangeAsync(serviceTypeKey, vehicleTypeKey);
                await _context.SaveChangesAsync();

                var values = new List<MasterDataValue>
                {
                    new MasterDataValue
                    {
                        MasterDataKeyId = serviceTypeKey.Id,
                        Value = "Oil Change",
                        CreatedBy = "System"
                    },
                    new MasterDataValue
                    {
                        MasterDataKeyId = serviceTypeKey.Id,
                        Value = "Brake Service",
                        CreatedBy = "System"
                    },
                    new MasterDataValue
                    {
                        MasterDataKeyId = vehicleTypeKey.Id,
                        Value = "Car",
                        CreatedBy = "System"
                    },
                    new MasterDataValue
                    {
                        MasterDataKeyId = vehicleTypeKey.Id,
                        Value = "Truck",
                        CreatedBy = "System"
                    }
                };

                await _context.MasterDataValues.AddRangeAsync(values);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminEmail = _adminSettings.Value.Email;
            var adminPassword = _adminSettings.Value.Password;
            var adminRole = _adminSettings.Value.Role;

            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await _userManager.CreateAsync(adminUser, adminPassword);
                await _userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}