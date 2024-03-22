using NetVisionProc.Common.Data.Interfaces;
using Throw;

namespace NetVisionProc.Common.Data
{
    public static class DbContextExt
    {
        public static void ApplySqlTransaction(this DbContext dbContext, ISqlScriptDef sqlScriptDef)
        {
            var sql = sqlScriptDef.GetSql();
            sql.Throw().IfEmpty();

            using (var transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.Database.ExecuteSqlRaw(sql);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public static async Task ApplySqlTransactionAsync(this DbContext dbContext, ISqlScriptDef sqlScriptDef)
        {
            var sql = sqlScriptDef.GetSql();
            sql.Throw().IfEmpty();

            await using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await dbContext.Database.ExecuteSqlRawAsync(sql);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        
        public static async Task<List<T>> GetListFromSqlRaw<T>(this DbContext dbContext, ISqlScriptDef sqlScriptDef)
        where T : class
        {
            var sql = sqlScriptDef.GetSql();
            sql.Throw().IfEmpty();

            try
            {
                var result = await dbContext.Database.SqlQueryRaw<T>(sql).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new DomainOperationException(
                    nameof(ApplySqlTransaction), 
                    $"An error occurred while executing SQL: {sql} with an error: {ex}");
            }
        }
        
        public static async Task<T?> GetFromSqlRaw<T>(this DbContext dbContext, ISqlScriptDef sqlScriptDef)
            where T : class
        {
            var listResult = await dbContext.GetListFromSqlRaw<T>(sqlScriptDef);
            return listResult.FirstOrDefault();
        }
        
        public static bool GetFirstBoolFromSqlRaw(this DbContext dbContext, ISqlScriptDef sqlScriptDef)
        {
            try
            {
                var sql = sqlScriptDef.GetSql();
                sql.Throw().IfEmpty();

                return dbContext.Database.SqlQueryRaw<bool>(sql).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new SqlOperationException(
                    nameof(GetFirstBoolFromSqlRaw),
                    $"An error occurred while executing SQL: {sqlScriptDef.GetSql()} with an error: {ex}");
            }
        }
    }
}