using Azure.Storage.Queues;

namespace NetVisionProc.AzureHub;

public class QueueStorageManager
{
    private readonly QueueClient _queueClient;

    public QueueStorageManager(string connectionString, string queueName)
    {
        _queueClient = new QueueClient(connectionString, queueName);
    }

    public async Task SendMessageAsync(string messageContent)
    {
        await _queueClient.CreateIfNotExistsAsync();

        await _queueClient.SendMessageAsync(messageContent);
    }
}