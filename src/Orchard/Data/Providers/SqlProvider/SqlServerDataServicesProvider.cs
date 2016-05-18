﻿using System;
using System.Data.Entity;

namespace Orchard.Data.Providers.SqlProvider {
    public class SqlServerDataServicesProvider : AbstractDataServicesProvider {
        public static string ProviderName {
            get { return "SqlServer"; }
        }

        protected override DbConfiguration GetConfiguration()
        {
            return new SqlServerConfiguration();
        }

        protected override string BuildConnectionString(SessionFactoryParameters parameters) {
            return parameters.ConnectionString;
        }
    }
}
