using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ASC.Web.Navigation
{
    public class NavigationCacheOperations : INavigationCacheOperations
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private const string NavigationCacheKey = "NavigationMenu";

        public NavigationCacheOperations(
            IWebHostEnvironment environment,
            IMemoryCache memoryCache)
        {
            _environment = environment;
            _memoryCache = memoryCache;
        }

        public async Task<List<NavigationMenuItem>> GetNavigationMenuAsync()
        {
            if (_memoryCache.TryGetValue(NavigationCacheKey, out List<NavigationMenuItem>? menuItems))
            {
                return menuItems ?? new List<NavigationMenuItem>();
            }

            var filePath = Path.Combine(_environment.ContentRootPath, "Navigation", "Navigation.json");

            if (!File.Exists(filePath))
            {
                return new List<NavigationMenuItem>();
            }

            var json = await File.ReadAllTextAsync(filePath);

            var navigationRoot = JsonSerializer.Deserialize<NavigationRoot>(json);

            menuItems = navigationRoot?.MenuItems
                .OrderBy(x => x.Sequence)
                .ToList() ?? new List<NavigationMenuItem>();

            _memoryCache.Set(NavigationCacheKey, menuItems);

            return menuItems;
        }

        public async Task<List<NavigationMenuItem>> GetNavigationMenuByRolesAsync(IList<string> roles)
        {
            var menuItems = await GetNavigationMenuAsync();

            return menuItems
                .Where(item => item.UserRoles.Any(role => roles.Contains(role)))
                .OrderBy(item => item.Sequence)
                .Select(item => new NavigationMenuItem
                {
                    DisplayName = item.DisplayName,
                    MaterialIcon = item.MaterialIcon,
                    Link = item.Link,
                    IsNested = item.IsNested,
                    Sequence = item.Sequence,
                    UserRoles = item.UserRoles,
                    NestedItems = item.NestedItems
                        .Where(child => child.UserRoles.Any(role => roles.Contains(role)))
                        .OrderBy(child => child.Sequence)
                        .ToList()
                })
                .ToList();
        }
    }
}