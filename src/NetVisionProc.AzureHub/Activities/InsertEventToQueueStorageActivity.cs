using System.Text.Json;
using Azure.Storage.Queues;

namespace NetVisionProc.AzureHub.Activities;

public class InsertEventToQueueStorageActivity
{
    private readonly QueueClient _queueClient;

    public InsertEventToQueueStorageActivity(QueueClient queueClient)
    {
        _queueClient = queueClient;
    }

    [FunctionName(nameof(InsertEventToQueueStorageActivity))]
    public async Task Run([ActivityTrigger] IDurableActivityContext context)
    {
        string blobName = context.GetInput<string>();

        await _queueClient.CreateIfNotExistsAsync();

        var message = new { BlobName = blobName };
        var messageJson = JsonSerializer.Serialize(message);
        
        await _queueClient.SendMessageAsync(messageJson);
    }
}