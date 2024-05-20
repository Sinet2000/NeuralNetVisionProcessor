using Azure;
using Azure.Data.Tables;

namespace NetVisionProc.Application.AzureTableScope;

public class AzureTableService(TableClient tableClient) : IAzureTableService
{
    public async Task<List<T>> GetListAsync<T>(string? filter = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new()
    {
        return await ListByFilter<T>(filter, select);
    }

    public async Task<List<T>> GetByPartitionAndRowAsync<T>(string partitionKey, string? rowKey = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new()
    {
        var results = new List<T>();
        try
        {
            await foreach (var page in tableClient.QueryAsync<T>(p => p.PartitionKey == partitionKey && p.RowKey == rowKey, select: select, maxPerPage: 1000).AsPages())
            {
                results.AddRange(page.Values.ToList());

                if (results.Count > 10000)
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return results;
    }

    public async Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new()
    {
        return await tableClient.GetEntityAsync<T>(partitionKey: partitionKey, rowKey: rowKey);
    }

    public async Task<Response> UpdateEntityAsync(ITableEntity tableEntity, ETag ifMatch)
    {
        return await tableClient.UpdateEntityAsync(tableEntity, ETag.All);
    }

    public async Task<Response> DeleteEntityAsync(string partitionKey, string rowKey)
    {
        return await tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    private async Task<List<T>> ListByFilter<T>(string? filter = null, IEnumerable<string>? select = null)
        where T : class, ITableEntity, new()
    {
        var results = new List<T>();
        try
        {
            await foreach (var page in tableClient.QueryAsync<T>(filter, select: select, maxPerPage: 1000).AsPages())
            {
                results.AddRange(page.Values.ToList());

                if (results.Count > 10000)
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return results;
    }
}

public interface IAzureTableService
{
    Task<List<T>> GetListAsync<T>(string? filter = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new();

    Task<List<T>> GetByPartitionAndRowAsync<T>(string partitionKey, string? rowKey = null, IEnumerable<string>? select = null) where T : class, ITableEntity, new();

    Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity, new();

    Task<Response> UpdateEntityAsync(ITableEntity tableEntity, ETag ifMatch);

    Task<Response> DeleteEntityAsync(string partitionKey, string rowKey);
}