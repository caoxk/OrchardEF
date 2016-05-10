using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Autofac.Features.Metadata;
using Orchard.Data;
using Orchard.Data.Providers;
using Orchard.Data.Providers.SqlProvider;
using Orchard.Environment.ShellBuilders.Models;

namespace Orchard.Tests.Data {
    public class ProviderUtilities {

        public static void RunWithSqlServer(IEnumerable<RecordBlueprint> recordDescriptors, Action<ISessionFactoryHolder> action) {
            var temporaryPath = Path.GetTempFileName();
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
            Directory.CreateDirectory(temporaryPath);
            var databasePath = Path.Combine(temporaryPath, "Orchard.mdf");
            var databaseName = Path.GetFileNameWithoutExtension(databasePath);
            try {
                // create database
                if (!TryCreateSqlServerDatabase(databasePath, databaseName))
                    return;

                var meta = new Meta<CreateDataServicesProvider>(() =>
                    new SqlServerDataServicesProvider(),
                    new Dictionary<string, object> { { "ProviderName", "SqlServer" } });

                var manager = (IDataServicesProviderFactory)new DataServicesProviderFactory(new[] { meta });

                var parameters = new SessionFactoryParameters {
                    Provider = "SqlServer",
                    DataFolder = temporaryPath,
                    ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFileName=" + databasePath + ";Integrated Security=True;User Instance=True;",
                    RecordDescriptors = recordDescriptors,
                };

                var provider = manager
                    .CreateProvider(parameters);
                var configuration = provider
                    .BuildConfiguration();
                DbConfiguration.SetConfiguration(configuration);
                var contextOptions = provider.GetContextOptions(parameters);

                var sessionFactory = new Stubs.StubSessionFactoryHolder(() => {
                    var session = new DbContext(contextOptions.ConnectionString, contextOptions.Model);
                    return session;
                });

                action(sessionFactory);
            }
            finally {
                try {
                    Directory.Delete(temporaryPath, true);
                }
                catch (IOException) { }
            }
        }

        private static bool TryCreateSqlServerDatabase(string databasePath, string databaseName) {
            var connection = TryOpenSqlServerConnection();
            if (connection == null)
                return false;

            using (connection) {
                using (var command = connection.CreateCommand()) {
                    command.CommandText =
                        "CREATE DATABASE " + databaseName +
                        " ON PRIMARY (NAME=" + databaseName +
                        ", FILENAME='" + databasePath.Replace("'", "''") + "')";
                    command.ExecuteNonQuery();

                    command.CommandText =
                        "EXEC sp_detach_db '" + databaseName + "', 'true'";
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        private static SqlConnection TryOpenSqlServerConnection() {
            try {
                var connection = new SqlConnection("Data Source=.\\SQLEXPRESS;Initial Catalog=tempdb;Integrated Security=true;User Instance=True;");
                connection.Open();
                return connection;
            }
            catch (SqlException e) {
                Trace.WriteLine(string.Format("Error opening connection to Sql Server ('{0}'). Skipping test.", e.Message));
                return null;
            }
        }

        public static void RunWithSqlCe(IEnumerable<RecordBlueprint> recordDescriptors, Action<ISessionFactoryHolder> action) {
            var temporaryPath = Path.GetTempFileName();
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
            Directory.CreateDirectory(temporaryPath);
            var databasePath = Path.Combine(temporaryPath, "Orchard.mdf");
            var databaseName = Path.GetFileNameWithoutExtension(databasePath);
            var parameters = new SessionFactoryParameters {
                Provider = "SqlServerCe",
                DataFolder = temporaryPath,
                RecordDescriptors = recordDescriptors
            };
            try {
                var manager = (IDataServicesProviderFactory)new DataServicesProviderFactory(new[] {
                new Meta<CreateDataServicesProvider>(
                    () => new Orchard.Data.Providers.SqlCeProvider.SqlServerCompactDataServicesProvider(databasePath),
                    new Dictionary<string, object> {{"ProviderName", "SqlServerCe" } })
            });
                var provider = manager
                    .CreateProvider(parameters);
                var configuration = provider
                    .BuildConfiguration();
                DbConfiguration.SetConfiguration(configuration);
                var contextOptions = provider.GetContextOptions(parameters);

                var sessionFactory = new Stubs.StubSessionFactoryHolder(() => {
                    var session = new DbContext(contextOptions.ConnectionString, contextOptions.Model);
                    return session;
                });
                action(sessionFactory);
            }
            finally {
                try {
                    Directory.Delete(temporaryPath, true);
                }
                catch (IOException) { }
            }
        }
    }
}
