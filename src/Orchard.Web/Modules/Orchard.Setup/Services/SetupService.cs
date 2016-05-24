using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Xml.Linq;
using Orchard.Core.Settings.Models;
using Orchard.Data;
using Orchard.Data.Migration;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Environment.ShellBuilders;
using Orchard.Environment.State;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Recipes.Models;
using Orchard.Recipes.Services;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Utility.Extensions;

namespace Orchard.Setup.Services {
    [OrchardFeature("Orchard.Setup.Services")]
    public class SetupService : Component, ISetupService {
        private readonly ShellSettings _shellSettings;
        private readonly IOrchardHost _orchardHost;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IShellContainerFactory _shellContainerFactory;
        private readonly ICompositionStrategy _compositionStrategy;
        private readonly IProcessingEngine _processingEngine;
        private readonly IRecipeHarvester _recipeHarvester;
        private IEnumerable<Recipe> _recipes;

        public SetupService(
            ShellSettings shellSettings,
            IOrchardHost orchardHost,
            IShellSettingsManager shellSettingsManager,
            IShellContainerFactory shellContainerFactory,
            ICompositionStrategy compositionStrategy,
            IProcessingEngine processingEngine,
            IRecipeHarvester recipeHarvester) {

            _shellSettings = shellSettings;
            _orchardHost = orchardHost;
            _shellSettingsManager = shellSettingsManager;
            _shellContainerFactory = shellContainerFactory;
            _compositionStrategy = compositionStrategy;
            _processingEngine = processingEngine;
            _recipeHarvester = recipeHarvester;
        }

        public ShellSettings Prime() {
            return _shellSettings;
        }

        public IEnumerable<Recipe> Recipes() {
            if (_recipes == null) {
                var recipes = new List<Recipe>();
                recipes.AddRange(_recipeHarvester.HarvestRecipes().Where(recipe => recipe.IsSetupRecipe));
                _recipes = recipes;
            }
            return _recipes;
        }

        public string Setup(SetupContext context) {
            var initialState = _shellSettings.State;
            try {
                return SetupInternal(context);
            }
            catch {
                _shellSettings.State = initialState;
                throw;
            }
        }

        private string SetupInternal(SetupContext context) {
            string executionId;

            Logger.Information("Running setup for tenant '{0}'.", _shellSettings.Name);

            // The vanilla Orchard distibution has the following features enabled.
            string[] hardcoded = {
                // Framework
                "Orchard.Framework",
                // Core
                 "Settings", "Shapes", "Navigation",
                // Modules
                "Orchard.Themes", "Orchard.Users", "Orchard.Roles", "Orchard.Modules", "Orchard.Recipes"//, "Orchard.Recipes"
            };

            context.EnabledFeatures = hardcoded.Union(context.EnabledFeatures ?? Enumerable.Empty<string>()).Distinct().ToList();

            // Set shell state to "Initializing" so that subsequent HTTP requests are responded to with "Service Unavailable" while Orchard is setting up.
            _shellSettings.State = TenantState.Initializing;

            var shellSettings = new ShellSettings(_shellSettings);

            if (String.IsNullOrEmpty(shellSettings.DataProvider)) {
                shellSettings.DataProvider = context.DatabaseProvider;
                shellSettings.DataConnectionString = context.DatabaseConnectionString;
                shellSettings.DataTablePrefix = context.DatabaseTablePrefix;
            }

            shellSettings.EncryptionAlgorithm = "AES";

            // Randomly generated key.
            shellSettings.EncryptionKey = SymmetricAlgorithm.Create(shellSettings.EncryptionAlgorithm).Key.ToHexString();
            shellSettings.HashAlgorithm = "HMACSHA256";

            // Randomly generated key.
            shellSettings.HashKey = HMAC.Create(shellSettings.HashAlgorithm).Key.ToHexString();

            var shellDescriptor = new ShellDescriptor {
                Features = context.EnabledFeatures.Select(name => new ShellFeature { Name = name })
            };

            var shellBlueprint = _compositionStrategy.Compose(shellSettings, shellDescriptor);

            // Initialize database explicitly, and store shell descriptor.
            using (var bootstrapLifetimeScope = _shellContainerFactory.CreateContainer(shellSettings, shellBlueprint)) {

                using (var environment = bootstrapLifetimeScope.CreateWorkContextScope()) {

                    // Check if the database is already created (in case an exception occured in the second phase).
                    var migrationExecutor = environment.Resolve<IMigrationExecutor>();
                    var installationPresent = true;
                    try {
                        var tablePrefix = String.IsNullOrEmpty(shellSettings.DataTablePrefix) ? "" : shellSettings.DataTablePrefix + "_";
                        installationPresent = migrationExecutor.TableExists(null, tablePrefix + "Settings_ShellDescriptorRecord");
                    }
                    catch (Exception ex) {
                        installationPresent = false;
                    }

                    if (installationPresent) {
                        throw new OrchardException(T("A previous Orchard installation was detected in this database with this table prefix."));
                    }

                    // Workaround to avoid some Transaction issue for PostgreSQL.
                    environment.Resolve<ITransactionManager>().RequireNew();

                    migrationExecutor.ExecuteMigration(builder => {
                        builder.Create.Table("Orchard_Framework_DataMigrationRecord")
                            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                            .WithColumn("DataMigrationClass").AsString()
                            .WithColumn("Version").AsInt32();

                        builder.Create
                            .UniqueConstraint("UC_DMR_DataMigrationClass_Version")
                            .OnTable("Orchard_Framework_DataMigrationRecord")
                            .Columns("DataMigrationClass", "Version");
                    });

                    var dataMigrationManager = environment.Resolve<IDataMigrationManager>();
                    dataMigrationManager.Update("Settings");

                    foreach (var feature in context.EnabledFeatures) {
                        dataMigrationManager.Update(feature);
                    }

                    environment.Resolve<IShellDescriptorManager>().UpdateShellDescriptor(
                        0,
                        shellDescriptor.Features,
                        shellDescriptor.Parameters);
                }
            }

            // In effect "pump messages" see PostMessage circa 1980.
            while (_processingEngine.AreTasksPending())
                _processingEngine.ExecuteNextTask();

            // Creating a standalone environment. 
            // in theory this environment can be used to resolve any normal components by interface, and those
            // components will exist entirely in isolation - no crossover between the safemode container currently in effect.
            using (var environment = _orchardHost.CreateStandaloneEnvironment(shellSettings)) {
                try {
                    executionId = CreateTenantData(context, environment);
                }
                catch {
                    environment.Resolve<ITransactionManager>().Cancel();
                    throw;
                }
            }

            _shellSettingsManager.SaveSettings(shellSettings);
            return executionId;
        }

        private string CreateTenantData(SetupContext context, IWorkContextScope environment) {
            // Create site owner.
            var membershipService = environment.Resolve<IMembershipService>();
            var user = membershipService.CreateUser(
                new CreateUserParams(context.AdminUsername, context.AdminPassword, 
                String.Empty, String.Empty, String.Empty, true));

            // Set site owner as current user for request (it will be set as the owner of all content items).
            var authenticationService = environment.Resolve<IAuthenticationService>();
            authenticationService.SetAuthenticatedUserForRequest(user);

            // Set site name and settings.
            var siteService = environment.Resolve<ISiteService>();
            var siteSettings = siteService.GetSiteSettings().As<SiteSettingsPart>();
            siteSettings.SiteSalt = Guid.NewGuid().ToString("N");
            siteSettings.SiteName = context.SiteName;
            siteSettings.SuperUser = context.AdminUsername;
            siteSettings.SiteCulture = "en-US";

            // Add default culture.
            var cultureManager = environment.Resolve<ICultureManager>();
            cultureManager.AddCulture("en-US");

            //var recipeManager = environment.Resolve<IRecipeManager>();
            //var recipe = context.Recipe;
            //var executionId = recipeManager.Execute(recipe);

            var recipeScheduler = environment.Resolve<IRecipeScheduler>();

            // Once the recipe has finished executing, we need to update the shell state to "Running", so add a recipe step that does exactly that.
            var recipeStepQueue = environment.Resolve<IRecipeStepQueue>();
            var recipeStepResultRecordRepository = environment.Resolve<IRepository<RecipeStepResultRecord>>();
            var activateShellStep = new RecipeStep(Guid.NewGuid().ToString("N"), "Setup", "ActivateShell", new XElement("ActivateShell"));
            recipeStepQueue.Enqueue(activateShellStep.Id, activateShellStep);
            recipeStepResultRecordRepository.Create(new RecipeStepResultRecord
            {
                ExecutionId = activateShellStep.Id,
                RecipeName = activateShellStep.RecipeName,
                StepId = activateShellStep.Id,
                StepName = activateShellStep.Name
            });
            recipeScheduler.ScheduleWork(activateShellStep.Id);

            // Null check: temporary fix for running setup in command line.
            if (HttpContext.Current != null) {
                authenticationService.SignIn(user, true);
            }

            return null;
        }
    }
}