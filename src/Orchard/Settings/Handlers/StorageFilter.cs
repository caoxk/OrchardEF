using System;
using Orchard.Data;
using Orchard.Settings.Records;

namespace Orchard.Settings.Handlers {
    public static class StorageFilter {
        public static StorageFilter<TRecord> For<TRecord>(IRepository<TRecord> repository) where TRecord : ContentPartRecord, new() {
            return new StorageFilter<TRecord>(repository);
        }
    }

    public class StorageFilter<TRecord> : StorageFilterBase<ContentPart<TRecord>> where TRecord : ContentPartRecord, new() {
        protected readonly IRepository<TRecord> _repository;

        public StorageFilter(IRepository<TRecord> repository) {
            _repository = repository;
        }

        protected virtual TRecord GetRecordCore(ContentItemRecord versionRecord) {
            return _repository.Get(versionRecord.Id);
        }

        protected virtual TRecord CreateRecordCore(ContentItemRecord versionRecord, TRecord record = null) {
            if (record == null) {
                record = new TRecord();
            }
            record.ContentItemRecord = versionRecord;
            _repository.Create(record);
            return record;
        }

        protected override void Activated(ActivatedContentContext context, ContentPart<TRecord> instance) {
            if (instance.Record != null) {
                throw new InvalidOperationException(string.Format(
                    "Having more than one storage filter for a given part ({0}) is invalid.",
                    typeof(ContentPart<TRecord>).FullName));
            }
            instance.Record = new TRecord();
        }

        protected override void Creating(CreateContentContext context, ContentPart<TRecord> instance) {
            CreateRecordCore(context.ContentItemRecord, instance.Record);
        }

        protected override void Loading(LoadContentContext context, ContentPart<TRecord> instance) {
            var versionRecord = context.ContentItemRecord;
            instance._record.Loader(() => GetRecordCore(versionRecord) ?? CreateRecordCore(versionRecord));
        }

        protected override void Destroying(DestroyContentContext context, ContentPart<TRecord> instance) {
            _repository.Delete(instance.Record);
        }
    }
}
