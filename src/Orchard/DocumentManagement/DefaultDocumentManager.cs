using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Autofac;
using Orchard.Caching;
using Orchard.Data;
using Orchard.DocumentManagement.Handlers;
using Orchard.DocumentManagement.Records;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Mvc;
using Orchard.UI;

namespace Orchard.DocumentManagement {
    public class DefaultDocumentManager : IDocumentManager {
        private readonly IComponentContext _context;
        private readonly IRepository<ContentItemRecord> _contentItemRepository;
        private readonly ICacheManager _cacheManager;
        private readonly Lazy<IDocumentDisplay> _contentDisplay;
        private readonly Lazy<ITransactionManager> _transactionManager; 
        private readonly Lazy<IEnumerable<IDocumentHandler>> _handlers;
        private readonly ShellSettings _shellSettings;
        private readonly ISignals _signals;

        private const string Published = "Published";
        private const string Draft = "Draft";

        public DefaultDocumentManager(
            IComponentContext context,
            IRepository<ContentItemRecord> contentItemRepository,
            ICacheManager cacheManager,
            Lazy<IDocumentDisplay> contentDisplay,
            Lazy<ITransactionManager> transactionManager,
            Lazy<IEnumerable<IDocumentHandler>> handlers,
            ShellSettings shellSettings,
            ISignals signals) {
            _context = context;
            _contentItemRepository = contentItemRepository;
            _cacheManager = cacheManager;
            _shellSettings = shellSettings;
            _signals = signals;
            _handlers = handlers;
            _contentDisplay = contentDisplay;
            _transactionManager = transactionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IEnumerable<IDocumentHandler> Handlers {
              get { return _handlers.Value; }
        }


        public virtual DocumentItem New(string contentType) {

            // create a new kernel for the model instance
            var context = new ActivatingDocumentContext {
                ContentType = contentType,
                Builder = new DocumentItemBuilder(contentType)
            };

            // invoke handlers to weld aspects onto kernel
            Handlers.Invoke(handler => handler.Activating(context), Logger);

            var context2 = new ActivatedDocumentContext {
                ContentType = contentType,
                ContentItem = context.Builder.Build()
            };

            // back-reference for convenience (e.g. getting metadata when in a view)
            context2.ContentItem.ContentManager = this;

            Handlers.Invoke(handler => handler.Activated(context2), Logger);

            var context3 = new InitializingDocumentContext {
                ContentType = context2.ContentType,
                ContentItem = context2.ContentItem,
            };

            Handlers.Invoke(handler => handler.Initializing(context3), Logger);
            Handlers.Invoke(handler => handler.Initialized(context3), Logger);

            // composite result is returned
            return context3.ContentItem;
        }

        public virtual DocumentItem Get(int id) {
            DocumentItem contentItem;

            ContentItemRecord itemRecord = _contentItemRepository.Get(id);

            // allocate instance and set record property
            contentItem = New(itemRecord.ContentType);
            contentItem.Record = itemRecord;
            
            // create a context with a new instance to load            
            var context = new LoadDocumentContext(contentItem);

            // invoke handlers to acquire state, or at least establish lazy loading callbacks
            Handlers.Invoke(handler => handler.Loading(context), Logger);
            Handlers.Invoke(handler => handler.Loaded(context), Logger);

            return contentItem;
        }

     
        //public virtual void Publish(ContentItem contentItem) {
        //    if (contentItem.VersionRecord.Published) {
        //        return;
        //    }
        //    // create a context for the item and it's previous published record
        //    var previous = contentItem.Record.Versions.SingleOrDefault(x => x.Published);
        //    var context = new PublishContentContext(contentItem, previous);

        //    // invoke handlers to acquire state, or at least establish lazy loading callbacks
        //    Handlers.Invoke(handler => handler.Publishing(context), Logger);

        //    if(context.Cancel) {
        //        return;
        //    }

        //    if (previous != null) {
        //        previous.Published = false;
        //    }
        //    contentItem.VersionRecord.Published = true;

        //    Handlers.Invoke(handler => handler.Published(context), Logger);
        //}

        //public virtual void Unpublish(ContentItem contentItem) {
        //    ContentItem publishedItem;
        //    if (contentItem.VersionRecord.Published) {
        //        // the version passed in is the published one
        //        publishedItem = contentItem;
        //    }
        //    else {
        //        // try to locate the published version of this item
        //        publishedItem = Get(contentItem.Id, VersionOptions.Published);
        //    }

        //    if (publishedItem == null) {
        //        // no published version exists. no work to perform.
        //        return;
        //    }

        //    // create a context for the item. the publishing version is null in this case
        //    // and the previous version is the one active prior to unpublishing. handlers
        //    // should take this null check into account
        //    var context = new PublishContentContext(contentItem, publishedItem.VersionRecord) {
        //        PublishingItemVersionRecord = null
        //    };

        //    Handlers.Invoke(handler => handler.Unpublishing(context), Logger);

        //    publishedItem.VersionRecord.Published = false;

        //    Handlers.Invoke(handler => handler.Unpublished(context), Logger);
        //}

        //public virtual void Remove(ContentItem contentItem) {
        //    var context = new RemoveContentContext(contentItem);
        //    Handlers.Invoke(handler => handler.Removing(context), Logger);

        //    Handlers.Invoke(handler => handler.Removed(context), Logger);
        //}

        public virtual void Destroy(DocumentItem contentItem) {
            var session = _transactionManager.Value.GetSession();
            //var context = new DestroyContentContext(contentItem);

            //// Give storage filters a chance to delete content part records.
            //Handlers.Invoke(handler => handler.Destroying(context), Logger);

            // Delete the content item record itself.
            session
                .Database
                .ExecuteSqlCommand("delete ContentItemRecord ci where ci.Id = @id)", contentItem.Id);

            //Handlers.Invoke(handler => handler.Destroyed(context), Logger);
        }

        public virtual void Create(DocumentItem contentItem) {

            if (contentItem.Record == null)
            {
                contentItem.Record = new ContentItemRecord
                {
                    ContentType = contentItem.ContentType
                };
            }
            _contentItemRepository.Create(contentItem.Record);

            // build a context with the initialized instance to create
            var context = new CreateDocumentContext(contentItem);

            // invoke handlers to add information to persistent stores
            Handlers.Invoke(handler => handler.Creating(context), Logger);

            Handlers.Invoke(handler => handler.Created(context), Logger);
        }
        
        public DocumentItemMetadata GetItemMetadata(IDocument content) {
            var context = new GetDocumentItemMetadataContext {
                ContentItem = content.ContentItem,
                Metadata = new DocumentItemMetadata()
            };

            Handlers.Invoke(handler => handler.GetContentItemMetadata(context), Logger);

            return context.Metadata;
        }

        public IEnumerable<GroupInfo> GetEditorGroupInfos(IDocument content) {
            var metadata = GetItemMetadata(content);
            return metadata.EditorGroupInfo
                .GroupBy(groupInfo => groupInfo.Id)
                .Select(grouping => grouping.OrderBy(groupInfo => groupInfo.Position, new FlatPositionComparer()).FirstOrDefault());
        }

        public IEnumerable<GroupInfo> GetDisplayGroupInfos(IDocument content) {
            var metadata = GetItemMetadata(content);
            return metadata.DisplayGroupInfo
                .GroupBy(groupInfo => groupInfo.Id)
                .Select(grouping => grouping.OrderBy(groupInfo => groupInfo.Position, new FlatPositionComparer()).FirstOrDefault());
        }

        public GroupInfo GetEditorGroupInfo(IDocument content, string groupInfoId) {
            return GetEditorGroupInfos(content).FirstOrDefault(gi => string.Equals(gi.Id, groupInfoId, StringComparison.OrdinalIgnoreCase));
        }

        public GroupInfo GetDisplayGroupInfo(IDocument content, string groupInfoId) {
            return GetDisplayGroupInfos(content).FirstOrDefault(gi => string.Equals(gi.Id, groupInfoId, StringComparison.OrdinalIgnoreCase));
        }

        public dynamic BuildDisplay(IDocument content, string displayType = "", string groupId = "") {
            return _contentDisplay.Value.BuildDisplay(content, displayType, groupId);
        }

        public dynamic BuildEditor(IDocument content, string groupId = "") {
            return _contentDisplay.Value.BuildEditor(content, groupId);
        }

        public dynamic UpdateEditor(IDocument content, IUpdateModel updater, string groupId = "") {
            var context = new UpdateContentContext(content.ContentItem);

            Handlers.Invoke(handler => handler.Updating(context), Logger);

            var result = _contentDisplay.Value.UpdateEditor(content, updater, groupId);

            Handlers.Invoke(handler => handler.Updated(context), Logger);

            return result;
        }

        public void Clear() {
        }

        public IEnumerable<DocumentItem> Query(string contentType)
        {
            var items = _contentItemRepository.Table
                .Where(x => x.ContentType == contentType)
                .ToList()
                .Select(x => Get(x.Id));
            return items;
        }
    }

    internal class CallSiteCollection : ConcurrentDictionary<string, CallSite<Func<CallSite, object, object>>> {
        private readonly Func<string, CallSite<Func<CallSite, object, object>>> _valueFactory;

        public CallSiteCollection(Func<string, CallSite<Func<CallSite, object, object>>> callSiteFactory) {
            _valueFactory = callSiteFactory;
        }

        public CallSiteCollection(Func<string, CallSiteBinder> callSiteBinderFactory) {
            _valueFactory = key => CallSite<Func<CallSite, object, object>>.Create(callSiteBinderFactory(key));
        }

        public object Invoke(object callee, string key) {
            var callSite = GetOrAdd(key, _valueFactory);
            return callSite.Target(callSite, callee);
        }
    }
}
