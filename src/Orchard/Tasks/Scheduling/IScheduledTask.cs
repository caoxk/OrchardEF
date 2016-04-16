using System;

namespace Orchard.Tasks.Scheduling {
    public interface IScheduledTask  {
        string TaskType { get; }
        DateTime? ScheduledUtc { get; }
    }
}