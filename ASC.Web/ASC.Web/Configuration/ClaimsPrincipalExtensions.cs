using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ASC.Web.Configuration
{
    public static class ClaimsPrincipalExtensions
    {
        public static CurrentUser GetCurrentUser(this ClaimsPrincipal principal)
        {
            var user = new CurrentUser
            {
                Id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                UserName = principal.Identity?.Name ?? string.Empty,
                Roles = principal.FindAll(ClaimTypes.Role)
                                 .Select(c => c.Value)
                                 .ToList()
            };

            return user;
        }
    }
}