using System.Globalization;

namespace Orchard.Localization {
    public static class LocalizationExtensions {
        public static CultureInfo CurrentCultureInfo(this WorkContext workContext) {
            return CultureInfo.GetCultureInfo(workContext.CurrentCulture);
        }
    }
}