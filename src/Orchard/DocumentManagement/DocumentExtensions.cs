using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Mvc;

namespace Orchard.DocumentManagement {
    public static class DocumentCreateExtensions
    {

        /* Item creation extension methods */

        public static T New<T>(this IDocumentManager manager, string contentType) where T : class, IDocument {
            var contentItem = manager.New(contentType);
            if (contentItem == null)
                return null;

            var part = contentItem.Get<T>();
            if (part == null)
                throw new InvalidCastException();

            return part;
        }

        public static void Create(this IDocumentManager manager, IDocument content) {
            manager.Create(content.ContentItem);
            //manager.Publish(content.ContentItem);
        }

        public static DocumentItem Create(this IDocumentManager manager, string contentType) {
            return manager.Create<DocumentItem>(contentType, init => { });
        }

        public static T Create<T>(this IDocumentManager manager, string contentType) where T : class, IDocument {
            return manager.Create<T>(contentType, init => { });
        }

        public static T Create<T>(this IDocumentManager manager, string contentType, Action<T> initialize) where T : class, IDocument {
            var content = manager.New<T>(contentType);
            if (content == null)
                return null;

            initialize(content);
            manager.Create(content.ContentItem);
            return content;
        }
    }

    public static class DocumentGetExtensions
    {


        public static T Get<T>(this IDocumentManager manager, int id) where T : class, IDocument {
            var contentItem = manager.Get(id);
            return contentItem == null ? null : contentItem.Get<T>();
        }
        
    }

    public static class DocumentExtensions {


        /* Display and editor convenience extension methods */

        public static TContent BuildDisplayShape<TContent>(this IDocumentManager manager, int id, string displayType) where TContent : class, IDocument {
            var content = manager.Get<TContent>(id);
            if (content == null)
                return null;
            return manager.BuildDisplay(content, displayType);
        }

        public static TContent BuildEditorShape<TContent>(this IDocumentManager manager, int id) where TContent : class, IDocument {
            var content = manager.Get<TContent>(id);
            if (content == null)
                return null;
            return manager.BuildEditor(content);

        }

        public static TContent UpdateEditorShape<TContent>(this IDocumentManager manager, int id, IUpdateModel updater) where TContent : class, IDocument {
            var content = manager.Get<TContent>(id);
            if (content == null)
                return null;
            return manager.UpdateEditor(content, updater);
        }




        /* Aggregate item/part type casting extension methods */

        public static bool Is<T>(this IDocument content) {
            return content == null ? false : content.ContentItem.Has(typeof(T));
        }
        public static T As<T>(this IDocument content) where T : IDocument {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }

        public static bool Has<T>(this IDocument content) {
            return content == null ? false : content.ContentItem.Has(typeof(T));
        }
        public static T Get<T>(this IDocument content) where T : IDocument {
            return content == null ? default(T) : (T)content.ContentItem.Get(typeof(T));
        }

        public static IEnumerable<T> AsPart<T>(this IEnumerable<DocumentItem> items) where T : IDocument {
            return items == null ? null : items.Where(item => item.Is<T>()).Select(item => item.As<T>());
        }
    }
}
