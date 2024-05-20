using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Config;
using NetVisionProc.AzureHub.TableEntities;

namespace NetVisionProc.AzureHub.Activities;

public class InsertDetectionResultToTableActivity
{
    private readonly AzureHubConfig _config;
    private readonly TableClient _tableClient;

    public InsertDetectionResultToTableActivity(AzureHubConfig config, TableClient tableClient)
    {
        _config = config;
        _tableClient = tableClient;
    }

    [FunctionName(nameof(InsertDetectionResultToTableActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        await _tableClient.CreateIfNotExistsAsync();
        var predictionResult = context.GetInput<ImagePredictionResult>();
            
        var processedBlobTableEntity = new ImagePredictionResultTableEntity(predictionResult);
        var entityToInsert = new TableEntity(processedBlobTableEntity.PartitionKey, processedBlobTableEntity.RowKey);
        foreach (var kvp in processedBlobTableEntity.GetPropertiesDictionary())
        {
            entityToInsert.Add(kvp.Key, kvp.Value);
        }
            
        await _tableClient.AddEntityAsync(entityToInsert);
    }
}