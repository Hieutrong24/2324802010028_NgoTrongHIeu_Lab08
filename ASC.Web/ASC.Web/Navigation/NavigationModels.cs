namespace ASC.Web.Navigation
{
    public class NavigationRoot
    {
        public List<NavigationMenuItem> MenuItems { get; set; } = new();
    }

    public class NavigationMenuItem
    {
        public string DisplayName { get; set; } = string.Empty;

        public string MaterialIcon { get; set; } = string.Empty;

        public string Link { get; set; } = string.Empty;

        public bool IsNested { get; set; }

        public int Sequence { get; set; }

        public List<string> UserRoles { get; set; } = new();

        public List<NavigationMenuItem> NestedItems { get; set; } = new();
    }
}