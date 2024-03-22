using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using NetVisionProc.Common.Data.Interfaces;

namespace NetVisionProc.Common.Data
{
    public static class MigrationBuilderExtensions
    {
        public static void EnableChangeDataCapture(
            this MigrationBuilder migrationBuilder,
            DbContext dbContext,
            Type entityType,
            ISqlScriptDef isEntityCdcEnabledSqlDef,
            ISqlScriptDef enableEntityCdcSqlDef)
        {
            string? migration = new StackFrame(1).GetMethod()?.DeclaringType?.Name;

            Guard.Against.NullOrEmpty(migration);
            
            string entityName = Setter.SetAndGuard(entityType.FullName);
            string? assemblyName = entityType.Assembly.GetName()?.Name;
            entityName += ", " + Setter.SetAndGuard(assemblyName);
            
            migrationBuilder.InsertData(
                table: "SeedMigrationsHistory",
                columns: ["MigrationName", "SeedName", "SeedFullTypeName", "SeedMethod", "StartedOn", "IsSucceeded", "IsError"],
                values: [migration, entityName, DateTime.UtcNow, false, false]);
            
            var isEntityCdcEnabled = dbContext.GetFirstBoolFromSqlRaw(isEntityCdcEnabledSqlDef);
            if (!isEntityCdcEnabled)
            {
                dbContext.ApplySqlTransaction(enableEntityCdcSqlDef);
            }
        }
        
        public static bool TableExists(this DbContext context, string tableName)
        {
            string sql = $"select count(*) as \"Value\" from information_schema.tables where table_name = '{tableName}'";
            int count = context.Database.SqlQueryRaw<int>(sql).ToList().First();

            return count > 0;
        }
    }
}