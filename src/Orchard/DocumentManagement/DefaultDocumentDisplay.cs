using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DocumentManagement.Handlers;
using Orchard.FileSystems.VirtualPath;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.UI.Zones;

namespace Orchard.DocumentManagement {
    public class DefaultDocumentDisplay : IDocumentDisplay {
        private readonly Lazy<IEnumerable<IDocumentHandler>> _handlers;
        private readonly IShapeFactory _shapeFactory;
        private readonly Lazy<IShapeTableLocator> _shapeTableLocator;

        private readonly RequestContext _requestContext;
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DefaultDocumentDisplay(
            Lazy<IEnumerable<IDocumentHandler>> handlers,
            IShapeFactory shapeFactory,
            Lazy<IShapeTableLocator> shapeTableLocator,
            RequestContext requestContext,
            IVirtualPathProvider virtualPathProvider,
            IWorkContextAccessor workContextAccessor) {
            _handlers = handlers;
            _shapeFactory = shapeFactory;
            _shapeTableLocator = shapeTableLocator;
            _requestContext = requestContext;
            _virtualPathProvider = virtualPathProvider;
            _workContextAccessor = workContextAccessor;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public dynamic BuildDisplay(IDocument content, string displayType, string groupId) {
            string stereotype = "Content";

            var actualShapeType = stereotype;
            var actualDisplayType = string.IsNullOrWhiteSpace(displayType) ? "Detail" : displayType;

            dynamic itemShape = CreateItemShape(actualShapeType);
            itemShape.ContentItem = content.ContentItem;
            itemShape.Metadata.DisplayType = actualDisplayType;

            var context = new BuildDisplayContext(itemShape, content, actualDisplayType, groupId, _shapeFactory);
            var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);
            context.Layout = workContext.Layout;

            BindPlacement(context, actualDisplayType, stereotype);

            _handlers.Value.Invoke(handler => handler.BuildDisplay(context), Logger);
            return context.Shape;
        }

        public dynamic BuildEditor(IDocument content, string groupId) {
            string stereotype = "Content";

            var actualShapeType = stereotype + "_Edit";

            dynamic itemShape = CreateItemShape(actualShapeType);
            itemShape.ContentItem = content.ContentItem;

            // adding an alternate for [Stereotype]_Edit__[ContentType] e.g. Content-Menu.Edit
            ((IShape)itemShape).Metadata.Alternates.Add(actualShapeType + "__" + content.ContentItem.ContentType);

            var context = new BuildEditorContext(itemShape, content, groupId, _shapeFactory);
            var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);
            context.Layout = workContext.Layout;
            BindPlacement(context, null, stereotype);

            _handlers.Value.Invoke(handler => handler.BuildEditor(context), Logger);


            return context.Shape;
        }

        public dynamic UpdateEditor(IDocument content, IUpdateModel updater, string groupInfoId) {
            string stereotype = "Content";

            var actualShapeType = stereotype + "_Edit";

            dynamic itemShape = CreateItemShape(actualShapeType);
            itemShape.ContentItem = content.ContentItem;

            var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);

            var theme = workContext.CurrentTheme;
            var shapeTable = _shapeTableLocator.Value.Lookup(theme.Id);

            // adding an alternate for [Stereotype]_Edit__[ContentType] e.g. Content-Menu.Edit
            ((IShape)itemShape).Metadata.Alternates.Add(actualShapeType + "__" + content.ContentItem.ContentType);

            var context = new UpdateEditorContext(itemShape, content, updater, groupInfoId, _shapeFactory, shapeTable, GetPath());
            context.Layout = workContext.Layout;
            BindPlacement(context, null, stereotype);

            _handlers.Value.Invoke(handler => handler.UpdateEditor(context), Logger);

            return context.Shape;
        }

        private dynamic CreateItemShape(string actualShapeType) {
            return _shapeFactory.Create(actualShapeType, Arguments.Empty(), () => new ZoneHolding(() => _shapeFactory.Create("ContentZone", Arguments.Empty())));
        }

        private void BindPlacement(BuildShapeContext context, string displayType, string stereotype) {
            context.FindPlacement = (partShapeType, differentiator, defaultLocation) => {

                var workContext = _workContextAccessor.GetContext(_requestContext.HttpContext);

                var theme = workContext.CurrentTheme;
                var shapeTable = _shapeTableLocator.Value.Lookup(theme.Id);

                ShapeDescriptor descriptor;
                if (shapeTable.Descriptors.TryGetValue(partShapeType, out descriptor)) {
                    var placementContext = new ShapePlacementContext {
                        ContentType = context.ContentItem.ContentType,
                        Stereotype = stereotype,
                        DisplayType = displayType,
                        Differentiator = differentiator,
                        Path = GetPath()
                    };

                    // define which location should be used if none placement is hit
                    descriptor.DefaultPlacement = defaultLocation;

                    var placement = descriptor.Placement(placementContext);
                    if (placement != null) {
                        placement.Source = placementContext.Source;
                        return placement;
                    }
                }

                return new PlacementInfo {
                    Location = defaultLocation,
                    Source = String.Empty
                };
            };
        }

        /// <summary>
        /// Gets the current app-relative path, i.e. ~/my-blog/foo.
        /// </summary>
        private string GetPath() {
            return VirtualPathUtility.AppendTrailingSlash(_virtualPathProvider.ToAppRelative(_requestContext.HttpContext.Request.Path));
        }
    }
}
