using System;
using Azure.Data.Tables;

namespace NetVisionProc.AzureHub.Activities;

public class InsertIntoTableStorageActivity
{
    private readonly TableClient _tableClient;

    public InsertIntoTableStorageActivity(TableClient tableClient)
    {
        _tableClient = tableClient;
    }
    
    [FunctionName(nameof(InsertIntoTableStorageActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context)
    {
        string blobName = context.GetInput<string>();
            
        await _tableClient.CreateIfNotExistsAsync();

        TableEntity tableEntity = new TableEntity(blobName, Guid.NewGuid().ToString())
        {
            { "Message", "Blob uploaded" }
        };

        // Insert the entity into the table
        await _tableClient.AddEntityAsync(tableEntity);
    }
}