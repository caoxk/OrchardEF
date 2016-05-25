using System;

namespace Orchard.Core.Scheduling.Models {
    public class ScheduledTaskRecord {
        public virtual int Id { get; set; }
        public virtual string TaskType { get; set; }
        public virtual DateTime? ScheduledUtc { get; set; }
    }
}