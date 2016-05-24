using Orchard.DocumentManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Orchard.Core.Settings {
    public class AdminMenu : INavigationProvider {
        private readonly IDocumentManager _contentManager;
        private readonly ISiteService _siteService;

        public AdminMenu(ISiteService siteService, IOrchardServices orchardServices, 
            IDocumentManager contentManager) {
            _siteService = siteService;
            Services = orchardServices;
            _contentManager = contentManager;
        }

        public Localizer T { get; set; }
        public string MenuName { get { return "admin"; } }
        public IOrchardServices Services { get; private set; }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("settings")
                .Add(T("Settings"), "99",
                    menu => menu.Add(T("General"), "0", item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = "Index" })
                        .Permission(Permissions.ManageSettings)), new [] {"collapsed"});

            var site = _siteService.GetSiteSettings();
            if (site == null)
                return;

            foreach (var groupInfo in _contentManager.GetEditorGroupInfos(site.ContentItem)) {
                GroupInfo info = groupInfo;
                builder.Add(T("Settings"),
                    menu => menu.Add(info.Name, info.Position, item => item.Action("Index", "Admin", new { area = "Settings", groupInfoId = info.Id })
                        .Permission(Permissions.ManageSettings)));
            }
        }
    }
}
