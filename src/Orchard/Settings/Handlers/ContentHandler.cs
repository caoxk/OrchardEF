using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;

namespace Orchard.ContentManagement.Handlers {
    public abstract class ContentHandler : IContentHandler {
        protected ContentHandler() {
            Filters = new List<IContentFilter>();
            Logger = NullLogger.Instance;
        }

        public List<IContentFilter> Filters { get; set; }
        public ILogger Logger { get; set; }

        protected void OnActivated<TPart>(Action<ActivatedContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnActivated = handler });
        }

        protected void OnInitializing<TPart>(Action<InitializingContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnInitializing = handler });
        }

        protected void OnInitialized<TPart>(Action<InitializingContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnInitialized = handler });
        }

        protected void OnCreating<TPart>(Action<CreateContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnCreating = handler });
        }

        protected void OnCreated<TPart>(Action<CreateContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnCreated = handler });
        }

        protected void OnLoading<TPart>(Action<LoadContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnLoading = handler });
        }

        protected void OnLoaded<TPart>(Action<LoadContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnLoaded = handler });
        }

        protected void OnUpdating<TPart>(Action<UpdateContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnUpdating = handler });
        }

        protected void OnUpdated<TPart>(Action<UpdateContentContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineStorageFilter<TPart> { OnUpdated = handler });
        }

        protected void OnGetContentItemMetadata<TPart>(Action<GetContentItemMetadataContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetItemMetadata = handler });
        }
        protected void OnGetDisplayShape<TPart>(Action<BuildDisplayContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetDisplayShape = handler });
        }

        protected void OnGetEditorShape<TPart>(Action<BuildEditorContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetEditorShape = handler });
        }

        protected void OnUpdateEditorShape<TPart>(Action<UpdateEditorContext, TPart> handler) where TPart : class, IContent {
            Filters.Add(new InlineTemplateFilter<TPart> { OnUpdateEditorShape = handler });
        }

        class InlineStorageFilter<TPart> : StorageFilterBase<TPart> where TPart : class, IContent {
            public Action<ActivatedContentContext, TPart> OnActivated { get; set; }
            public Action<InitializingContentContext, TPart> OnInitializing { get; set; }
            public Action<InitializingContentContext, TPart> OnInitialized { get; set; }
            public Action<CreateContentContext, TPart> OnCreating { get; set; }
            public Action<CreateContentContext, TPart> OnCreated { get; set; }
            public Action<LoadContentContext, TPart> OnLoading { get; set; }
            public Action<LoadContentContext, TPart> OnLoaded { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdating { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdated { get; set; }
            protected override void Activated(ActivatedContentContext context, TPart instance) {
                if (OnActivated != null) OnActivated(context, instance);
            }
            protected override void Initializing(InitializingContentContext context, TPart instance) {
                if (OnInitializing != null) OnInitializing(context, instance);
            }
            protected override void Initialized(InitializingContentContext context, TPart instance) {
                if (OnInitialized != null) OnInitialized(context, instance);
            }
            protected override void Creating(CreateContentContext context, TPart instance) {
                if (OnCreating != null) OnCreating(context, instance);
            }
            protected override void Created(CreateContentContext context, TPart instance) {
                if (OnCreated != null) OnCreated(context, instance);
            }
            protected override void Loading(LoadContentContext context, TPart instance) {
                if (OnLoading != null) OnLoading(context, instance);
            }
            protected override void Loaded(LoadContentContext context, TPart instance) {
                if (OnLoaded != null) OnLoaded(context, instance);
            }
            protected override void Updating(UpdateContentContext context, TPart instance) {
                if (OnUpdating != null) OnUpdating(context, instance);
            }
            protected override void Updated(UpdateContentContext context, TPart instance) {
                if (OnUpdated != null) OnUpdated(context, instance);
            }
        }

        class InlineTemplateFilter<TPart> : TemplateFilterBase<TPart> where TPart : class, IContent {
            public Action<GetContentItemMetadataContext, TPart> OnGetItemMetadata { get; set; }
            public Action<BuildDisplayContext, TPart> OnGetDisplayShape { get; set; }
            public Action<BuildEditorContext, TPart> OnGetEditorShape { get; set; }
            public Action<UpdateEditorContext, TPart> OnUpdateEditorShape { get; set; }
            protected override void GetContentItemMetadata(GetContentItemMetadataContext context, TPart instance) {
                if (OnGetItemMetadata != null) OnGetItemMetadata(context, instance);
            }
            protected override void BuildDisplayShape(BuildDisplayContext context, TPart instance) {
                if (OnGetDisplayShape != null) OnGetDisplayShape(context, instance);
            }
            protected override void BuildEditorShape(BuildEditorContext context, TPart instance) {
                if (OnGetEditorShape != null) OnGetEditorShape(context, instance);
            }
            protected override void UpdateEditorShape(UpdateEditorContext context, TPart instance) {
                if (OnUpdateEditorShape != null) OnUpdateEditorShape(context, instance);
            }
        }

        void IContentHandler.Activating(ActivatingContentContext context) {
            foreach (var filter in Filters.OfType<IContentActivatingFilter>())
                filter.Activating(context);
            Activating(context);
        }

        void IContentHandler.Activated(ActivatedContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Activated(context);
            Activated(context);
        }

        void IContentHandler.Initializing(InitializingContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Initializing(context);
            Initializing(context);
        }

        void IContentHandler.Initialized(InitializingContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Initialized(context);
            Initialized(context);
        }
        
        void IContentHandler.Creating(CreateContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Creating(context);
            Creating(context);
        }

        void IContentHandler.Created(CreateContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Created(context);
            Created(context);
        }

        void IContentHandler.Loading(LoadContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Loading(context);
            Loading(context);
        }

        void IContentHandler.Loaded(LoadContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Loaded(context);
            Loaded(context);
        }

        void IContentHandler.Updating(UpdateContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Updating(context);
            Updating(context);
        }

        void IContentHandler.Updated(UpdateContentContext context) {
            foreach (var filter in Filters.OfType<IContentStorageFilter>())
                filter.Updated(context);
            Updated(context);
        }

        void IContentHandler.GetContentItemMetadata(GetContentItemMetadataContext context) {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.GetContentItemMetadata(context);
            GetItemMetadata(context);
        }
        void IContentHandler.BuildDisplay(BuildDisplayContext context) {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.BuildDisplayShape(context);
            BuildDisplayShape(context);
        }
        void IContentHandler.BuildEditor(BuildEditorContext context) {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.BuildEditorShape(context);
            BuildEditorShape(context);
        }
        void IContentHandler.UpdateEditor(UpdateEditorContext context) {
            foreach (var filter in Filters.OfType<IContentTemplateFilter>())
                filter.UpdateEditorShape(context);
            UpdateEditorShape(context);
        }

        protected virtual void Activating(ActivatingContentContext context) { }
        protected virtual void Activated(ActivatedContentContext context) { }

        protected virtual void Initializing(InitializingContentContext context) { }
        protected virtual void Initialized(InitializingContentContext context) { }

        protected virtual void Creating(CreateContentContext context) { }
        protected virtual void Created(CreateContentContext context) { }

        protected virtual void Loading(LoadContentContext context) { }
        protected virtual void Loaded(LoadContentContext context) { }

        protected virtual void Updating(UpdateContentContext context) { }
        protected virtual void Updated(UpdateContentContext context) { }

        protected virtual void GetItemMetadata(GetContentItemMetadataContext context) { }
        protected virtual void BuildDisplayShape(BuildDisplayContext context) { }
        protected virtual void BuildEditorShape(BuildEditorContext context) { }
        protected virtual void UpdateEditorShape(UpdateEditorContext context) { }
    }
}