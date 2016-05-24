using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;

namespace Orchard.DocumentManagement.Handlers {
    public abstract class DocumentHandler : IDocumentHandler {
        protected DocumentHandler() {
            Filters = new List<IDocumentFilter>();
            Logger = NullLogger.Instance;
        }

        public List<IDocumentFilter> Filters { get; set; }
        public ILogger Logger { get; set; }

        protected void OnActivated<TPart>(Action<ActivatedDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnActivated = handler });
        }

        protected void OnInitializing<TPart>(Action<InitializingDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnInitializing = handler });
        }

        protected void OnInitialized<TPart>(Action<InitializingDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnInitialized = handler });
        }

        protected void OnCreating<TPart>(Action<CreateDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnCreating = handler });
        }

        protected void OnCreated<TPart>(Action<CreateDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnCreated = handler });
        }

        protected void OnLoading<TPart>(Action<LoadDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnLoading = handler });
        }

        protected void OnLoaded<TPart>(Action<LoadDocumentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnLoaded = handler });
        }

        protected void OnUpdating<TPart>(Action<UpdateContentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnUpdating = handler });
        }

        protected void OnUpdated<TPart>(Action<UpdateContentContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineStorageFilter<TPart> { OnUpdated = handler });
        }

        protected void OnGetContentItemMetadata<TPart>(Action<GetDocumentItemMetadataContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetItemMetadata = handler });
        }
        protected void OnGetDisplayShape<TPart>(Action<BuildDisplayContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetDisplayShape = handler });
        }

        protected void OnGetEditorShape<TPart>(Action<BuildEditorContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineTemplateFilter<TPart> { OnGetEditorShape = handler });
        }

        protected void OnUpdateEditorShape<TPart>(Action<UpdateEditorContext, TPart> handler) where TPart : class, IDocument {
            Filters.Add(new InlineTemplateFilter<TPart> { OnUpdateEditorShape = handler });
        }

        class InlineStorageFilter<TPart> : StorageFilterBase<TPart> where TPart : class, IDocument {
            public Action<ActivatedDocumentContext, TPart> OnActivated { get; set; }
            public Action<InitializingDocumentContext, TPart> OnInitializing { get; set; }
            public Action<InitializingDocumentContext, TPart> OnInitialized { get; set; }
            public Action<CreateDocumentContext, TPart> OnCreating { get; set; }
            public Action<CreateDocumentContext, TPart> OnCreated { get; set; }
            public Action<LoadDocumentContext, TPart> OnLoading { get; set; }
            public Action<LoadDocumentContext, TPart> OnLoaded { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdating { get; set; }
            public Action<UpdateContentContext, TPart> OnUpdated { get; set; }
            protected override void Activated(ActivatedDocumentContext context, TPart instance) {
                if (OnActivated != null) OnActivated(context, instance);
            }
            protected override void Initializing(InitializingDocumentContext context, TPart instance) {
                if (OnInitializing != null) OnInitializing(context, instance);
            }
            protected override void Initialized(InitializingDocumentContext context, TPart instance) {
                if (OnInitialized != null) OnInitialized(context, instance);
            }
            protected override void Creating(CreateDocumentContext context, TPart instance) {
                if (OnCreating != null) OnCreating(context, instance);
            }
            protected override void Created(CreateDocumentContext context, TPart instance) {
                if (OnCreated != null) OnCreated(context, instance);
            }
            protected override void Loading(LoadDocumentContext context, TPart instance) {
                if (OnLoading != null) OnLoading(context, instance);
            }
            protected override void Loaded(LoadDocumentContext context, TPart instance) {
                if (OnLoaded != null) OnLoaded(context, instance);
            }
            protected override void Updating(UpdateContentContext context, TPart instance) {
                if (OnUpdating != null) OnUpdating(context, instance);
            }
            protected override void Updated(UpdateContentContext context, TPart instance) {
                if (OnUpdated != null) OnUpdated(context, instance);
            }
        }

        class InlineTemplateFilter<TPart> : TemplateFilterBase<TPart> where TPart : class, IDocument {
            public Action<GetDocumentItemMetadataContext, TPart> OnGetItemMetadata { get; set; }
            public Action<BuildDisplayContext, TPart> OnGetDisplayShape { get; set; }
            public Action<BuildEditorContext, TPart> OnGetEditorShape { get; set; }
            public Action<UpdateEditorContext, TPart> OnUpdateEditorShape { get; set; }
            protected override void GetContentItemMetadata(GetDocumentItemMetadataContext context, TPart instance) {
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

        void IDocumentHandler.Activating(ActivatingDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentActivatingFilter>())
                filter.Activating(context);
            Activating(context);
        }

        void IDocumentHandler.Activated(ActivatedDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Activated(context);
            Activated(context);
        }

        void IDocumentHandler.Initializing(InitializingDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Initializing(context);
            Initializing(context);
        }

        void IDocumentHandler.Initialized(InitializingDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Initialized(context);
            Initialized(context);
        }
        
        void IDocumentHandler.Creating(CreateDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Creating(context);
            Creating(context);
        }

        void IDocumentHandler.Created(CreateDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Created(context);
            Created(context);
        }

        void IDocumentHandler.Loading(LoadDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Loading(context);
            Loading(context);
        }

        void IDocumentHandler.Loaded(LoadDocumentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Loaded(context);
            Loaded(context);
        }

        void IDocumentHandler.Updating(UpdateContentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Updating(context);
            Updating(context);
        }

        void IDocumentHandler.Updated(UpdateContentContext context) {
            foreach (var filter in Filters.OfType<IDocumentStorageFilter>())
                filter.Updated(context);
            Updated(context);
        }

        void IDocumentHandler.GetContentItemMetadata(GetDocumentItemMetadataContext context) {
            foreach (var filter in Filters.OfType<IDocumentTemplateFilter>())
                filter.GetContentItemMetadata(context);
            GetItemMetadata(context);
        }
        void IDocumentHandler.BuildDisplay(BuildDisplayContext context) {
            foreach (var filter in Filters.OfType<IDocumentTemplateFilter>())
                filter.BuildDisplayShape(context);
            BuildDisplayShape(context);
        }
        void IDocumentHandler.BuildEditor(BuildEditorContext context) {
            foreach (var filter in Filters.OfType<IDocumentTemplateFilter>())
                filter.BuildEditorShape(context);
            BuildEditorShape(context);
        }
        void IDocumentHandler.UpdateEditor(UpdateEditorContext context) {
            foreach (var filter in Filters.OfType<IDocumentTemplateFilter>())
                filter.UpdateEditorShape(context);
            UpdateEditorShape(context);
        }

        protected virtual void Activating(ActivatingDocumentContext context) { }
        protected virtual void Activated(ActivatedDocumentContext context) { }

        protected virtual void Initializing(InitializingDocumentContext context) { }
        protected virtual void Initialized(InitializingDocumentContext context) { }

        protected virtual void Creating(CreateDocumentContext context) { }
        protected virtual void Created(CreateDocumentContext context) { }

        protected virtual void Loading(LoadDocumentContext context) { }
        protected virtual void Loaded(LoadDocumentContext context) { }

        protected virtual void Updating(UpdateContentContext context) { }
        protected virtual void Updated(UpdateContentContext context) { }

        protected virtual void GetItemMetadata(GetDocumentItemMetadataContext context) { }
        protected virtual void BuildDisplayShape(BuildDisplayContext context) { }
        protected virtual void BuildEditorShape(BuildEditorContext context) { }
        protected virtual void UpdateEditorShape(UpdateEditorContext context) { }
    }
}