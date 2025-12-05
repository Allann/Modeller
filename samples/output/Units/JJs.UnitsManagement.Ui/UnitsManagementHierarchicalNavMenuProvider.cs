using MudBlazor;
using Shell.Plugin;

namespace JJs.UnitsManagement.Ui;

/// <summary>
/// Hierarchical navigation provider for the Units Management UI plugin, organizing units management pages into logical groups.
/// </summary>
public class UnitsManagementHierarchicalNavMenuProvider : IHierarchicalNavMenuProvider
{
    /// <summary>
    /// Gets the hierarchical navigation menu items for the Units Management UI plugin.
    /// </summary>
    /// <returns>A collection of hierarchical navigation menu items organized by units management categories.</returns>
    public IEnumerable<IHierarchicalNavMenuItem> GetHierarchicalNavMenuItems()
    {
        return HierarchicalNavMenuBuilder.Create()
            // Main Units Management Group
            .AddGroup(
                id: "units-management",
                text: "Units Management",
                icon: Icons.Material.Filled.LocalShipping,
                order: 1000,
                isExpandedByDefault: false,
                requiresAuthorisation: true,
                requiredRoles: new[] { "Plugin.Units.Administrator", "Plugin.Units.ReadWrite", "Plugin.Units.Read" })

            // Dashboard Link under Units Management
            .AddLink(
                id: "units-dashboard",
                text: "Dashboard",
                href: "/units/home",
                icon: Icons.Material.Filled.Dashboard,
                order: 1001,
                parentId: "units-management")

            // Units List Link
            .AddLink(
                id: "units-list",
                text: "Units List",
                href: "/units",
                icon: Icons.Material.Filled.List,
                order: 1002,
                parentId: "units-management")

            // Manage Subgroup
            .AddGroup(
                id: "units-manage",
                text: "Manage",
                icon: Icons.Material.Filled.Build,
                order: 1003,
                parentId: "units-management",
                isExpandedByDefault: true)

            .AddLink(
                id: "all-units",
                text: "All Units",
                href: "/units",
                icon: Icons.Material.Filled.LocalShipping,
                order: 1004,
                parentId: "units-manage")

            .Build();
    }
}
