using System.Collections.Generic;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using NetVisionProc.AzureHub.Activities.Models;
using NetVisionProc.AzureHub.Config;
using NetVisionProc.AzureHub.TableEntities;

namespace NetVisionProc.AzureHub.Activities;

public class InsertDetectionResultsToTableActivity
{
    private readonly AzureHubConfig _config;
    private readonly TableClient _tableClient;

    public InsertDetectionResultsToTableActivity(AzureHubConfig config, TableClient tableClient)
    {
        _config = config;
        _tableClient = tableClient;
    }

    [FunctionName(nameof(InsertDetectionResultsToTableActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context, ILogger log)
    {
        await _tableClient.CreateIfNotExistsAsync();
        var predictionResults = context.GetInput<List<ImagePredictionResult>>();
            
        var processedBlobTableEntity = new ImagePredictionResultTableEntity(predictionResults);
        var entityToInsert = new TableEntity(processedBlobTableEntity.PartitionKey, processedBlobTableEntity.RowKey);
        foreach (var kvp in processedBlobTableEntity.GetPropertiesDictionary())
        {
            entityToInsert.Add(kvp.Key, kvp.Value);
        }
            
        await _tableClient.AddEntityAsync(entityToInsert);
    }
}