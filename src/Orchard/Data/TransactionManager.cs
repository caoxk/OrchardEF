﻿using System.Data;
using System.Web.Mvc;
using Microsoft.Data.Entity;
using Orchard.Mvc.Filters;

namespace Orchard.Data {
    public interface ITransactionManager : IDependency {
        void Demand();
        void RequireNew();
        void RequireNew(IsolationLevel level);
        void Cancel();

        DbContext GetSession();
    }

    public class TransactionFilter : FilterProvider, IExceptionFilter {
        private readonly ITransactionManager _transactionManager;

        public TransactionFilter(ITransactionManager transactionManager) {
            _transactionManager = transactionManager;
        }

        public void OnException(ExceptionContext filterContext) {
            _transactionManager.Cancel();
        }
    }
}
