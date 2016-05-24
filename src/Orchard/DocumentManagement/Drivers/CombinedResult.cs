using System.Collections.Generic;
using System.Linq;
using Orchard.DocumentManagement.Handlers;

namespace Orchard.DocumentManagement.Drivers {
    public class CombinedResult : DriverResult {
        private readonly IEnumerable<DriverResult> _results;

        public CombinedResult(IEnumerable<DriverResult> results) {
            _results = results.Where(x => x != null);
        }

        public override void Apply(BuildDisplayContext context) {
            foreach (var result in _results) {

                // copy the ContentPart which was used to render this result to its children
                // so they can assign it to the concrete shapes
                if (result.ContentPart == null && ContentPart != null) {
                    result.ContentPart = ContentPart;
                }

                result.Apply(context);
            }
        }

        public override void Apply(BuildEditorContext context) {
            foreach (var result in _results) {

                // copy the ContentPart which was used to render this result to its children
                // so they can assign it to the concrete shapes
                if (result.ContentPart == null && ContentPart != null) {
                    result.ContentPart = ContentPart;
                }
                
                result.Apply(context);
            }
        }

        public IEnumerable<DriverResult> GetResults() {
            return _results;
        } 
    }
}