using Orchard.Data.Migration;

namespace Orchard.Recipes {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.Create
                .Table("RecipeStepResultRecord")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("ExecutionId").AsString(128)
                .WithColumn("RecipeName").AsString(256)
                .WithColumn("StepId").AsString(32).NotNullable()
                .WithColumn("StepName").AsString(256).NotNullable()
                .WithColumn("IsCompleted").AsBoolean()
                .WithColumn("IsSuccessful").AsBoolean()
                .WithColumn("ErrorMessage").AsString(int.MaxValue);

            SchemaBuilder.Create
                .UniqueConstraint("IDX_RecipeStepResultRecord_ExecutionId").OnTable("RecipeStepResultRecord").Column("ExecutionId");
            SchemaBuilder.Create
                .UniqueConstraint("IDX_RecipeStepResultRecord_ExecutionId_StepName").OnTable("RecipeStepResultRecord").Columns("ExecutionId", "StepName");
            
            return 1;
        }
    }
}