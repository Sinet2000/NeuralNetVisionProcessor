using Azure;
using Azure.Data.Tables;
using ClientApp.Models;
using Newtonsoft.Json;

namespace ClientApp.TableEntities
{
    public class ProcessedBlobTableEntity : ITableEntity
    {
        public ProcessedBlobTableEntity()
        {
        }

        public ProcessedBlobTableEntity(ProcessedBlobModel processedBlob)
        {
            Guard.Against.NullOrEmpty(processedBlob.FileName);
            PartitionKey = processedBlob.FileName;
            RowKey = RT.Comb.Provider.Sql.GetTimestamp(RT.Comb.Provider.Sql.Create()).ToString("o");
            ProcessedBlob = processedBlob;
        }

        public string PartitionKey { get; set; } = null!;
        
        public string RowKey { get; set; } = null!;
        
        private string ProcessedBlobJson { get; set; } = null!;

        private ProcessedBlobModel ProcessedBlob { get; set; } = null!;

        // Implement required members of ITableEntity
        public DateTimeOffset? Timestamp { get; set; }
        
        public ETag ETag { get; set; }

        public void ReadEntity(IDictionary<string, object> properties, string operationContext)
        {
            // Deserialize JSON data when reading from table
            if (properties.TryGetValue(nameof(ProcessedBlobJson), out var jsonValue) && jsonValue is string jsonString)
            {
                ProcessedBlobJson = jsonString;
                ProcessedBlob = JsonConvert.DeserializeObject<ProcessedBlobModel>(ProcessedBlobJson);
            }
        }

        public IDictionary<string, object> GetPropertiesDictionary()
        {
            ProcessedBlobJson = JsonConvert.SerializeObject(ProcessedBlob);
            var dictionary = new Dictionary<string, object>
            {
                { nameof(ProcessedBlobJson), ProcessedBlobJson }
            };
            return dictionary;
        }
    }
}