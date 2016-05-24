using System;
using Orchard.Data;
using Orchard.DocumentManagement.Records;

namespace Orchard.DocumentManagement.Handlers {
    public static class StorageFilter {
        public static StorageFilter<TRecord> For<TRecord>(IRepository<TRecord> repository) where TRecord : DocumentPartRecord, new() {
            return new StorageFilter<TRecord>(repository);
        }
    }

    public class StorageFilter<TRecord> : StorageFilterBase<ContentPart<TRecord>> where TRecord : DocumentPartRecord, new() {
        protected readonly IRepository<TRecord> _repository;

        public StorageFilter(IRepository<TRecord> repository) {
            _repository = repository;
        }

        protected virtual TRecord GetRecordCore(DocumentItemRecord versionRecord) {
            return _repository.Get(versionRecord.Id);
        }

        protected virtual TRecord CreateRecordCore(DocumentItemRecord versionRecord, TRecord record = null) {
            if (record == null) {
                record = new TRecord();
            }
            record.ContentItemRecord = versionRecord;
            _repository.Create(record);
            return record;
        }

        protected override void Activated(ActivatedDocumentContext context, ContentPart<TRecord> instance) {
            if (instance.Record != null) {
                throw new InvalidOperationException(string.Format(
                    "Having more than one storage filter for a given part ({0}) is invalid.",
                    typeof(ContentPart<TRecord>).FullName));
            }
            instance.Record = new TRecord();
        }

        protected override void Creating(CreateDocumentContext context, ContentPart<TRecord> instance) {
            CreateRecordCore(context.ContentItemRecord, instance.Record);
        }

        protected override void Loading(LoadDocumentContext context, ContentPart<TRecord> instance) {
            var versionRecord = context.ContentItemRecord;
            instance._record.Loader(() => GetRecordCore(versionRecord) ?? CreateRecordCore(versionRecord));
        }

        protected override void Destroying(DestroyDocumentContext context, ContentPart<TRecord> instance) {
            _repository.Delete(instance.Record);
        }
    }
}
