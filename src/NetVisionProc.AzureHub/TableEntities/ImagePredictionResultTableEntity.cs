using System;
using System.Collections.Generic;
using Azure;
using Azure.Data.Tables;
using NetVisionProc.AzureHub.Activities.Models;
using Newtonsoft.Json;

namespace NetVisionProc.AzureHub.TableEntities
{
    public class ImagePredictionResultTableEntity : ITableEntity
    {
        public ImagePredictionResultTableEntity()
        {
        }

        public ImagePredictionResultTableEntity(List<ImagePredictionResult> predictionResults)
        {
            PartitionKey = RT.Comb.Provider.Sql.GetTimestamp(RT.Comb.Provider.Sql.Create()).ToString("o");
            RowKey = RT.Comb.Provider.Sql.GetTimestamp(RT.Comb.Provider.Sql.Create()).ToString("o");
            PredictionResults = predictionResults;
        }

        public string PartitionKey { get; set; }
        
        public string RowKey { get; set; }
        
        private string PredictionResultJson { get; set; }

        public List<ImagePredictionResult> PredictionResults { get; set; }

        // Implement required members of ITableEntity
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public void ReadEntity(IDictionary<string, object> properties, string operationContext)
        {
            // Deserialize JSON data when reading from table
            if (properties.TryGetValue(nameof(PredictionResultJson), out var jsonValue) && jsonValue is string jsonString)
            {
                PredictionResultJson = jsonString;
                PredictionResults = JsonConvert.DeserializeObject<List<ImagePredictionResult>>(PredictionResultJson) ?? new List<ImagePredictionResult>();
            }
        }

        public IDictionary<string, object> GetPropertiesDictionary()
        {
            PredictionResultJson = JsonConvert.SerializeObject(PredictionResults);
            var dictionary = new Dictionary<string, object>
            {
                { nameof(PredictionResultJson), PredictionResultJson }
            };
            
            return dictionary;
        }
    }
}