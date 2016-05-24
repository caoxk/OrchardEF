using Orchard.DisplayManagement;

namespace Orchard.DocumentManagement.Handlers {
    public class BuildEditorContext : BuildShapeContext {
        public BuildEditorContext(IShape model, IDocument content, string groupId, IShapeFactory shapeFactory)
            : base(model, content, groupId, shapeFactory) {
        }
    }
}