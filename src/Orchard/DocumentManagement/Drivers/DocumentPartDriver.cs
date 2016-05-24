using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;
using Orchard.DocumentManagement.Extensions;
using Orchard.DocumentManagement.Handlers;
using Orchard.DocumentManagement.MetaData;
using Orchard.Mvc;

namespace Orchard.DocumentManagement.Drivers {
    public abstract class DocumentPartDriver<TContent> : IDocumentPartDriver where TContent : DocumentPart, new() {
        protected virtual string Prefix { get { return typeof(TContent).Name; } }

        void IDocumentPartDriver.GetContentItemMetadata(GetDocumentItemMetadataContext context) {
            var part = context.ContentItem.As<TContent>();
            if (part != null)
                GetContentItemMetadata(part, context.Metadata);
        }

        DriverResult IDocumentPartDriver.BuildDisplay(BuildDisplayContext context) {
            var part = context.ContentItem.As<TContent>();

            if (part == null) {
                return null;
            }

            DriverResult result = Display(part, context.DisplayType, context.New);

            if (result != null) {
                result.ContentPart = part;
            }

            return result;
        }

        DriverResult IDocumentPartDriver.BuildEditor(BuildEditorContext context) {
            var part = context.ContentItem.As<TContent>();

            if (part == null) {
                return null;
            }

            DriverResult result = Editor(part, context.New);

            if (result != null) {
                result.ContentPart = part;
            }

            return result;
        }

        DriverResult IDocumentPartDriver.UpdateEditor(UpdateEditorContext context) {
            var part = context.ContentItem.As<TContent>();

            if (part == null) {
                return null;
            }

            // Checking if the editor needs to be updated (e.g. if any of the shapes were not hidden).
            DriverResult editor = Editor(part, context.New);
            IEnumerable<DocumentShapeResult> contentShapeResults = editor.GetShapeResults();

            if (contentShapeResults.Any(contentShapeResult =>
                contentShapeResult == null || contentShapeResult.WasDisplayed(context))) {
                DriverResult result = Editor(part, context.Updater, context.New);

                if (result != null) {
                    result.ContentPart = part;
                }

                return result;
            }

            return editor;
        }

        

        protected virtual void GetContentItemMetadata(TContent context, DocumentItemMetadata metadata) { }

        protected virtual DriverResult Display(TContent part, string displayType, dynamic shapeHelper) { return null; }
        protected virtual DriverResult Editor(TContent part, dynamic shapeHelper) { return null; }
        protected virtual DriverResult Editor(TContent part, IUpdateModel updater, dynamic shapeHelper) { return null; }

        

        [Obsolete("Provided while transitioning to factory variations")]
        public DocumentShapeResult ContentShape(IShape shape) {
            return ContentShapeImplementation(shape.Metadata.Type, ctx => shape).Location("Content");
        }

        public DocumentShapeResult ContentShape(string shapeType, Func<dynamic> factory) {
            return ContentShapeImplementation(shapeType, ctx => factory());
        }

        public DocumentShapeResult ContentShape(string shapeType, Func<dynamic, dynamic> factory) {
            return ContentShapeImplementation(shapeType, ctx => factory(CreateShape(ctx, shapeType)));
        }

        private DocumentShapeResult ContentShapeImplementation(string shapeType, Func<BuildShapeContext, object> shapeBuilder) {
            return new DocumentShapeResult(shapeType, Prefix, ctx => {
                var shape = shapeBuilder(ctx);

                if (shape == null) {
                    return null;
                }

                return AddAlternates(shape, ctx); ;
            });
        }

        private static dynamic AddAlternates(dynamic shape, BuildShapeContext ctx) {
            ShapeMetadata metadata = shape.Metadata;

            // if no ContentItem property has been set, assign it
            if (shape.ContentItem == null) {
                shape.ContentItem = ctx.ContentItem;
            }

            var shapeType = metadata.Type;

            // [ShapeType]__[Id] e.g. Parts/Common.Metadata-42
            metadata.Alternates.Add(shapeType + "__" + ctx.ContentItem.Id.ToString(CultureInfo.InvariantCulture));

            return shape;
        }

        private static object CreateShape(BuildShapeContext context, string shapeType) {
            IShapeFactory shapeFactory = context.New;
            return shapeFactory.Create(shapeType);
        }

        public CombinedResult Combined(params DriverResult[] results) {
            return new CombinedResult(results);
        }

        public IEnumerable<ContentPartInfo> GetPartInfo() {
            var contentPartInfo = new[] {
                new ContentPartInfo {
                    PartName = typeof (TContent).Name,
                    Factory = () => new TContent {}
                }
            };

            return contentPartInfo;
        }

    }
}