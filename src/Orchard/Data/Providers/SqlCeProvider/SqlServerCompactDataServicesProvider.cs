using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Orchard.Data.Providers.SqlCeProvider {
    public class SqlServerCompactDataServicesProvider : IDataServicesProvider {
        public static string ProviderName
        {
            get { return "SqlServerCe"; }
        }

        public void ConfigureContextOptions(DbContextOptionsBuilder optionsBuilders, string dataFolder, string connectionString) {
            if (!string.IsNullOrEmpty(dataFolder))
                Directory.CreateDirectory(dataFolder);

            var fileName = Path.Combine(dataFolder, "Orchard.sdf");

            string localConnectionString = string.Format("Data Source={0}", fileName);
            if (!File.Exists(fileName)) {
                CreateSqlCeDatabaseFile(localConnectionString);
            }
            optionsBuilders.UseSqlCe(localConnectionString);
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
