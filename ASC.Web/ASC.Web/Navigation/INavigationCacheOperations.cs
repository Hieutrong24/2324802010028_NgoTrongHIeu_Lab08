namespace ASC.Web.Navigation
{
    public interface INavigationCacheOperations
    {
        Task<List<NavigationMenuItem>> GetNavigationMenuAsync();

        Task<List<NavigationMenuItem>> GetNavigationMenuByRolesAsync(IList<string> roles);
    }
}