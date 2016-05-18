using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Data
{
    public class DataContext : DbContext
    {
        private readonly IDataStoreEventHandler _dataStoreEvents;

        public DataContext(IDataStoreEventHandler dataStoreEvents, string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            _dataStoreEvents = dataStoreEvents;
        }

        public override int SaveChanges()
        {
            _dataStoreEvents.PreSaveChanges(this);
            var rowAffecteds =  base.SaveChanges();
            _dataStoreEvents.PostSaveChanges();
            return rowAffecteds;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            _dataStoreEvents.PreSaveChanges(this);
            var rowAffecteds = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _dataStoreEvents.PostSaveChanges();

            return rowAffecteds;
        }
    }
}
