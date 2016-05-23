﻿namespace Orchard.Settings.Handlers {
    public interface IContentHandler : IDependency {
        void Activating(ActivatingContentContext context);
        void Activated(ActivatedContentContext context);
        void Initializing(InitializingContentContext context);
        void Initialized(InitializingContentContext context);
        void Creating(CreateContentContext context);
        void Created(CreateContentContext context);
        void Loading(LoadContentContext context);
        void Loaded(LoadContentContext context);
        void Updating(UpdateContentContext context);
        void Updated(UpdateContentContext context);
        //void Publishing(PublishContentContext context);
        //void Published(PublishContentContext context);
        //void Unpublishing(PublishContentContext context);
        //void Unpublished(PublishContentContext context);
        //void Removing(RemoveContentContext context);
        //void Removed(RemoveContentContext context);

        void GetContentItemMetadata(GetContentItemMetadataContext context);
        void BuildDisplay(BuildDisplayContext context);
        void BuildEditor(BuildEditorContext context);
        void UpdateEditor(UpdateEditorContext context);
    }
}