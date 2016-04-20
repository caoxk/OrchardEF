using System;

namespace Orchard.Settings {
    /// <summary>
    /// Interface provided by the "settings" model.
    /// </summary>
    public interface ISite  {
        string PageTitleSeparator { get; }
        string SiteName { get; set; }
        string SiteSalt { get; set; }
        string SuperUser { get; set; }
        string HomePage { get; set; }
        string SiteCulture { get; set; }
        string SiteCalendar { get; set; }
        ResourceDebugMode ResourceDebugMode { get; set; }
        bool UseCdn { get; set; }
        int PageSize { get; set; }
        int MaxPageSize { get; set; }
        int MaxPagedCount { get; set; }
        string BaseUrl { get; set; }
        string SiteTimeZone { get; set; }
    }
}
