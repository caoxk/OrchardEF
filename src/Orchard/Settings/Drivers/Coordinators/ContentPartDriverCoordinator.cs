using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Logging;

namespace Orchard.ContentManagement.Drivers.Coordinators {
    /// <summary>
    /// This component coordinates how parts are taking part in the rendering when some content needs to be rendered.
    /// It will dispatch BuildDisplay/BuildEditor to all <see cref="IContentPartDriver"/> implementations.
    /// </summary>
    public class ContentPartDriverCoordinator : ContentHandlerBase {
        private readonly IEnumerable<IContentPartDriver> _drivers;

        public ContentPartDriverCoordinator(IEnumerable<IContentPartDriver> drivers) {
            _drivers = drivers;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public override void Activating(ActivatingContentContext context) {

            var partInfos = _drivers.SelectMany(cpp => cpp.GetPartInfo()).ToList();

            foreach (var partInfo in partInfos) {
                var part = partInfo.Factory();
                context.Builder.Weld(part);
            }
        }

        public override void GetContentItemMetadata(GetContentItemMetadataContext context) {
            _drivers.Invoke(driver => driver.GetContentItemMetadata(context), Logger);
        }

        public override void BuildDisplay(BuildDisplayContext context) {
            _drivers.Invoke(driver => {
                var result = driver.BuildDisplay(context);
                if (result != null)
                    result.Apply(context);
            }, Logger);
        }

        public override void BuildEditor(BuildEditorContext context) {
            _drivers.Invoke(driver => {
                var result = driver.BuildEditor(context);
                if (result != null)
                    result.Apply(context);
            }, Logger);
        }

        public override void UpdateEditor(UpdateEditorContext context) {
            _drivers.Invoke(driver => {
                var result = driver.UpdateEditor(context);
                if (result != null)
                    result.Apply(context);
            }, Logger);
        }
    }
}