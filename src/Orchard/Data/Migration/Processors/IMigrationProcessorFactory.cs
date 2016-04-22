using FluentMigrator;

namespace Orchard.Data.Migration.Processors
{
    public interface IMigrationProcessorFactory: IDependency
    {
        IMigrationProcessor Create(IAnnouncer announcer, IMigrationProcessorOptions options);

        bool IsForProvider(string provider);

        string Name { get; }
    }
}