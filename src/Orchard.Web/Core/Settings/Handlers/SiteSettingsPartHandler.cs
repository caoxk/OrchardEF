using System;
using Orchard.Core.Settings.Models;
using Orchard.DocumentManagement.Handlers;

namespace Orchard.Core.Settings.Handlers {
    public class SiteSettingsPartHandler : DocumentHandler {
        public SiteSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<SiteSettingsPart>("Site"));

            OnInitializing<SiteSettingsPart>(InitializeSiteSettings);
        }

        private static void InitializeSiteSettings(InitializingDocumentContext initializingContentContext, SiteSettingsPart siteSettingsPart) {
            siteSettingsPart.SiteSalt = Guid.NewGuid().ToString("N");
            siteSettingsPart.SiteName = "My Orchard Project Application";
            siteSettingsPart.PageTitleSeparator = " - ";
            siteSettingsPart.SiteTimeZone = TimeZoneInfo.Local.Id;
        }
    }
}