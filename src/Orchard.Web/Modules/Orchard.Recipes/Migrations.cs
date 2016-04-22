using Orchard.Data.Migration;

namespace Orchard.Recipes {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.Create
                .Table("Orchard_Recipes_RecipeStepResultRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("ExecutionId").AsString(128)
                .WithColumn("RecipeName").AsString(256)
                .WithColumn("StepId").AsString(32).NotNullable()
                .WithColumn("StepName").AsString(256).NotNullable()
                .WithColumn("IsCompleted").AsBoolean()
                .WithColumn("IsSuccessful").AsBoolean()
                .WithColumn("ErrorMessage").AsString(int.MaxValue).Nullable();

            SchemaBuilder.Create
                .UniqueConstraint("IDX_RecipeStepResultRecord_ExecutionId").OnTable("Orchard_Recipes_RecipeStepResultRecord").Column("ExecutionId");
            SchemaBuilder.Create
                .UniqueConstraint("IDX_RecipeStepResultRecord_ExecutionId_StepName").OnTable("Orchard_Recipes_RecipeStepResultRecord").Columns("ExecutionId", "StepName");
            
            return 1;
        }
    }
}