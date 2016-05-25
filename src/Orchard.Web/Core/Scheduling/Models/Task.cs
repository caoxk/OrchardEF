using System;
using Orchard.Tasks.Scheduling;

namespace Orchard.Core.Scheduling.Models {
    public class Task : IScheduledTask {
        private readonly ScheduledTaskRecord _record;
        private bool _itemInitialized;

        public Task(ScheduledTaskRecord record) {
            // in spite of appearances, this is actually a created class, not IoC, 
            // but dependencies are passed in for lazy initialization purposes
            _record = record;
        }

        public string TaskType {
            get { return _record.TaskType; }
        }

        public DateTime? ScheduledUtc {
            get { return _record.ScheduledUtc; }
        }
    }
}