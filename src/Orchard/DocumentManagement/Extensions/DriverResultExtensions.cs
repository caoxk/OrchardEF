using System.Collections.Generic;
using System.Linq;
using Orchard.DocumentManagement.Drivers;

namespace Orchard.DocumentManagement.Extensions
{
    internal static class DriverResultExtensions {
        public static IEnumerable<DocumentShapeResult> GetShapeResults(this DriverResult driverResult) {
            if (driverResult is CombinedResult) {
                return ((CombinedResult)driverResult).GetResults().Select(result => result as DocumentShapeResult);
            }

            return new[] { driverResult as DocumentShapeResult };
        }
    }
}