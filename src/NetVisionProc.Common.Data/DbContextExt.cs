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
        {
            var sql = sqlScriptDef.GetSql();
            sql.Throw().IfEmpty();

            try
            {
                await using var command = dbContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;

                await dbContext.Database.OpenConnectionAsync();
                await using var reader = await command.ExecuteReaderAsync();

                var resultList = new List<T>();
                while (await reader.ReadAsync())
                {
                    var instance = Activator.CreateInstance<T>();
                    var properties = typeof(T).GetProperties();
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i < properties.Length)
                        {
                            properties[i].SetValue(instance, values[i]);
                        }
                    }

                    resultList.Add(instance);
                }

                return resultList;
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
    }
}