using FluentMigrator;

namespace Orchard.Data.Migration.Processors
{
    public class ProcessorOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public int Timeout { get; set; }
        public string ProviderSwitches  { get; set; }
    }
}