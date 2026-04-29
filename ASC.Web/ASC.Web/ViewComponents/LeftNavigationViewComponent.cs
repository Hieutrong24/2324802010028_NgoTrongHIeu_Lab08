using ASC.Utilities;
using ASC.Web.Configuration;
using ASC.Web.Navigation;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.ViewComponents
{
    public class LeftNavigationViewComponent : ViewComponent
    {
        private readonly INavigationCacheOperations _navigationCacheOperations;

        public LeftNavigationViewComponent(INavigationCacheOperations navigationCacheOperations)
        {
            _navigationCacheOperations = navigationCacheOperations;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = HttpContext.Session.GetSession<CurrentUser>("CurrentUser");

            var roles = currentUser?.Roles ?? new List<string>();

            var menuItems = await _navigationCacheOperations.GetNavigationMenuByRolesAsync(roles);

            return View(menuItems);
        }
    }
}