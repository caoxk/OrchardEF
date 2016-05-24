using System.Linq;
using Orchard.Caching;
using Orchard.Core.Settings.Models;
using Orchard.DocumentManagement;
using Orchard.Settings;

namespace Orchard.Core.Settings.Services {
    public class SiteService : ISiteService {
        private readonly IDocumentManager _contentManager;
        private readonly ICacheManager _cacheManager;

        public SiteService(
            IDocumentManager contentManager,
            ICacheManager cacheManager) {
            _contentManager = contentManager;
            _cacheManager = cacheManager;
        }

        public ISite GetSiteSettings() {
            var siteId = _cacheManager.Get("SiteId", true, ctx => {
                var site = _contentManager.Query("Site")
                    .FirstOrDefault();

                if (site == null) {
                    site = _contentManager.Create<SiteSettingsPart>("Site").ContentItem;
                }

                return site.Id;
            });

            return _contentManager.Get<ISite>(siteId);
        }
    }
}