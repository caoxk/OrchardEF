﻿using Orchard.Settings;

namespace Orchard.Core.Settings.Models {
    public class SiteSettingsPartRecord : ISite {
        public virtual int Id { get; set; }
        public virtual string PageTitleSeparator { get; set; }
        public string SiteName {
            get; set;
        }

        public string SiteSalt {
            get; set;
        }

        public string SuperUser {
            get; set;
        }

        public string HomePage {
            get; set;
        }

        public string SiteCulture {
            get; set;
        }

		public string SiteCalendar {
            get; set;
        }

        public ResourceDebugMode ResourceDebugMode {
            get; set;
        }
        
        public bool UseCdn {
            get; set;
        }

        public int PageSize {
            get; set;
        }

        public int MaxPageSize {
            get; set;
        }

        public int MaxPagedCount {
            get; set;
        }

        public string SiteTimeZone {
            get; set;
        }

        public string BaseUrl {
            get; set;
        }
    }
}
