using Azure.Data.Tables;
using NetVisionProc.Application.AzureTableScope;
using NetVisionProc.Application.AzureTableScope.Entities;
using NetVisionProc.Application.AzureTableScope.Models;

namespace ClientApp.Services;

public class NetVisioProcessorService : INetVisioProcessorService
{
    private readonly IAzureTableService _azTableService;

    public NetVisioProcessorService(IAzureTableService azTableService)
    {
        _azTableService = azTableService;
    }

    public async Task<List<ImagePredictionResult>> ListPredictions()
    {
        var filters = new List<string>();

        var result = await _azTableService
            .GetListAsync<ImagePredictionResultTableEntity>(
                string.Empty,
                select: new [] { nameof(ImagePredictionResultTableEntity.PredictionResult) });
        result.ForEach(r => r.MaterializeJson());

        return result.Select(c => c.PredictionResult).ToList();
    }
}

public interface INetVisioProcessorService
{
    
}