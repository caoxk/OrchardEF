using Orchard.Data;

namespace Orchard.Tests.Settings
{
    public static class DataContextExtensions
    {
        public static void Clear(this DataContext context)
        {
            foreach (var entityEntry in context.ChangeTracker.Entries())
            {
                ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext.Detach(entityEntry.Entity);
            }
        }
    }
}
