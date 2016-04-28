using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data.Providers.SqlProvider;

namespace Orchard.Data.Providers.SqlCeProvider {
    public class SqlServerCompactDataServicesProvider : IDataServicesProvider {
        public static string ProviderName
        {
            get { return "SqlServerCe"; }
        }

        public DbConfiguration BuildConfiguration()
        {
            return new SqlCeConfiguration();
        }

        public DbContextOptions GetContextOptions(SessionFactoryParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.DataFolder))
                Directory.CreateDirectory(parameters.DataFolder);

            var fileName = Path.Combine(parameters.DataFolder, "Orchard.sdf");

            string localConnectionString = string.Format("Data Source={0}", fileName);
            if (!File.Exists(fileName))
            {
                CreateSqlCeDatabaseFile(localConnectionString);
            }
            return new DbContextOptions { ConnectionString = localConnectionString };
        }

        private void CreateSqlCeDatabaseFile(string connectionString) {
            // We want to execute this code using Reflection, to avoid having a binary
            // dependency on SqlCe assembly

            //engine engine = new SqlCeEngine();
            //const string assemblyName = "System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            const string assemblyName = "System.Data.SqlServerCe";
            const string typeName = "System.Data.SqlServerCe.SqlCeEngine";

            var sqlceEngineHandle = Activator.CreateInstance(assemblyName, typeName);
            var engine = sqlceEngineHandle.Unwrap();

            //engine.LocalConnectionString = connectionString;
            engine.GetType().GetProperty("LocalConnectionString").SetValue(engine, connectionString, null/*index*/);

            //engine.CreateDatabase();
            engine.GetType().GetMethod("CreateDatabase").Invoke(engine, null);

            //engine.Dispose();
            engine.GetType().GetMethod("Dispose").Invoke(engine, null);
        }
    }
}
