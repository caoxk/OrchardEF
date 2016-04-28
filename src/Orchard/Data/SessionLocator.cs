using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Orchard.Exceptions;
using Orchard.Logging;
using Orchard.Security;
using System.Data.Entity;

namespace Orchard.Data {

    public class SessionLocator : ISessionLocator {
        private readonly ITransactionManager _transactionManager;

        public SessionLocator(ITransactionManager transactionManager) {
            _transactionManager = transactionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public DbContext For(Type entityType) {
            Logger.Debug("Acquiring session for {0}", entityType);
            return _transactionManager.GetSession();
        }
    }

    public class TransactionManager : ITransactionManager, IDisposable {
        private DbContext _dataContext;
        private ISessionFactoryHolder _sessionFactoryHolder;
        //private Func<DataContext> _dataContextFactory;

        public TransactionManager(
            //Func<DataContext> dataContextFactory, 
            ISessionFactoryHolder sessionFactoryHolder) {
            _sessionFactoryHolder = sessionFactoryHolder;

            //_dataContextFactory = dataContextFactory;

            Logger = NullLogger.Instance;
            IsolationLevel = IsolationLevel.ReadCommitted;
        }

        public ILogger Logger { get; set; }
        public IsolationLevel IsolationLevel { get; set; }

        public DbContext GetSession() {
            Demand();
            return _dataContext;
        }
        public void Demand() {
            EnsureSession(IsolationLevel);
        }

        public void RequireNew() {
            RequireNew(IsolationLevel);
        }

        public void RequireNew(IsolationLevel level) {
            DisposeSession();
            EnsureSession(level);
        }

        public void Cancel() {
            if (_dataContext != null) {
                _dataContext.Database.CurrentTransaction?.Rollback();
                _dataContext.Database.UseTransaction(null);
                DisposeSession();
            }
        }

        public void Dispose() {
            DisposeSession();
        }

        private void DisposeSession() {
            if (_dataContext != null) {
                try {
                    _dataContext.SaveChanges();
                    if (_dataContext.Database.CurrentTransaction != null) {
                        Logger.Debug("Committing transaction");
                        _dataContext.Database.CurrentTransaction.Commit();
                    }
                }
                finally {
                    Logger.Debug("Disposing session");
                    _dataContext.Dispose();
                    _dataContext = null;
                }
            }
        }

        private void EnsureSession(IsolationLevel level) {
            if (_dataContext != null) {
                return;
            }
            _dataContext = _sessionFactoryHolder.Create();
            _dataContext.Database.BeginTransaction(level);
        }
    }
}