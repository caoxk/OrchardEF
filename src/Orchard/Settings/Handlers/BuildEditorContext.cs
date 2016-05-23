using Orchard.DisplayManagement;

namespace Orchard.Settings.Handlers {
    public class BuildEditorContext : BuildShapeContext {
        public BuildEditorContext(IShape model, IContent content, string groupId, IShapeFactory shapeFactory)
            : base(model, content, groupId, shapeFactory) {
        }
    }
}