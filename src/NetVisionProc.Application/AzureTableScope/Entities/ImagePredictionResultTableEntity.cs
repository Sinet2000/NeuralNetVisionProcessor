using Azure;
using Azure.Data.Tables;
using NetVisionProc.Application.AzureTableScope.Models;
using Newtonsoft.Json;

namespace NetVisionProc.Application.AzureTableScope.Entities
{
    public class ImagePredictionResultTableEntity : ITableEntity
    {
        public ImagePredictionResultTableEntity()
        {
        }

        public ImagePredictionResultTableEntity(ImagePredictionResult predictionResult)
        {
            PartitionKey = RT.Comb.Provider.Sql.GetTimestamp(RT.Comb.Provider.Sql.Create()).ToString("o");
            RowKey = RT.Comb.Provider.Sql.GetTimestamp(RT.Comb.Provider.Sql.Create()).ToString("o");
            PredictionResult = predictionResult;
        }

        public string PartitionKey { get; set; }
        
        public string RowKey { get; set; }
        
        public string PredictionResultJson { get; set; }

        public ImagePredictionResult? PredictionResult { get; set; }

        // Implement required members of ITableEntity
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public void ReadEntity(IDictionary<string, object> properties, string operationContext)
        {
            // Deserialize JSON data when reading from table
            if (properties.TryGetValue(nameof(PredictionResultJson), out var jsonValue) && jsonValue is string jsonString)
            {
                PredictionResultJson = jsonString;
                PredictionResult = JsonConvert.DeserializeObject<ImagePredictionResult>(PredictionResultJson);
            }
        }

        public IDictionary<string, object> GetPropertiesDictionary()
        {
            PredictionResultJson = JsonConvert.SerializeObject(PredictionResult);
            var dictionary = new Dictionary<string, object>
            {
                { nameof(PredictionResultJson), PredictionResultJson }
            };
            
            return dictionary;
        }

        public void MaterializeJson()
        {
            PredictionResult = JsonConvert.DeserializeObject<ImagePredictionResult>(PredictionResultJson);
        }
    }
}