namespace ClientApp.Config
{
    public class AzureHubConfig
    {
        public const string SectionName = "AzureHubConfig";

        public string? DbConnectionString { get; set; }

        public string BlobContainerName { get; set; } = null!;

        public string ProcessedBlobsContainerName { get; set; } = null!;

        public string TableStorageName { get; set; } = null!;

        public string QueueName { get; set; } = null!;

        public string BlobConnectionString { get; set; } = null!;

        public string TableConnectionString { get; set; } = null!;

        public string QueueConnectionString { get; set; } = null!;
    }
}