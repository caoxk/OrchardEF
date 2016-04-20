using System;
using System.Linq;
using Orchard.Caching;
using Orchard.Core.Settings.Models;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Settings;

namespace Orchard.Core.Settings.Services {
    public class SiteService : ISiteService {       
        private readonly ICacheManager _cacheManager;

        public SiteService(
            IRepository<SiteSettingsPartRecord> siteSettingsRepository,            
            ICacheManager cacheManager) {          
            _cacheManager = cacheManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ISite GetSiteSettings() {
            var item = new SiteSettingsPartRecord();
            item.Id = 1;
            item.SiteSalt = "ca66c4cf061441efaf838cb99fe3126d";
            item.SiteName = "Test Site";
            item.SuperUser = "admin";
            item.PageTitleSeparator = " - ";
            item.HomePage = "OrchardLocal";
            item.SiteCulture = "en-US";
            item.ResourceDebugMode = ResourceDebugMode.FromAppSetting;
            item.PageSize = 10;
            item.SiteTimeZone = "China Standard Time";
            return item;
        }
    }
}