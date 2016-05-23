﻿namespace Orchard.Settings.Handlers {
    public class ContentHandlerBase : IContentHandler {
        public virtual void Activating(ActivatingContentContext context) {}
        public virtual void Activated(ActivatedContentContext context) {}
        public virtual void Initializing(InitializingContentContext context) { }
        public virtual void Initialized(InitializingContentContext context) { }
        public virtual void Creating(CreateContentContext context) { }
        public virtual void Created(CreateContentContext context) {}
        public virtual void Loading(LoadContentContext context) {}
        public virtual void Loaded(LoadContentContext context) {}
        public virtual void Updating(UpdateContentContext context) { }
        public virtual void Updated(UpdateContentContext context) { }

        public virtual void GetContentItemMetadata(GetContentItemMetadataContext context) {}
        public virtual void BuildDisplay(BuildDisplayContext context) {}
        public virtual void BuildEditor(BuildEditorContext context) {}
        public virtual void UpdateEditor(UpdateEditorContext context) {}
    }
}