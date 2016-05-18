using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Events;

namespace Orchard.Data
{
    public interface IDataStoreEventHandler : IEventHandler
    {
        void PostSaveChanges();
        void PreSaveChanges(DataContext context);
    }
}
