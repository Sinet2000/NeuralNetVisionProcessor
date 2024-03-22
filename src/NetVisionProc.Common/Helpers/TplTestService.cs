// namespace EvoCDCTest.Common.Helpers
// {
//     using System.Diagnostics;
//     using EvoCDCTest.Common;
//     using TPL.Models;
//
//     // https://github.com/mjrousos/MultiThreadedEFCoreSample/blob/master/SampleWebApi/Controllers/BooksController.cs
//     public class TplTestService : ITplTestService
//     {
//         private readonly IEntityDbContext<EvoAppDbContext> _db;
//         private readonly Stopwatch _timer = new();
//         private readonly ILogger<TplTestService> _logger;
//         private readonly IServiceScopeFactory _scopeFactory;
//         private const int ParallelizationFactor = 100;
//
//         public TplTestService(IEntityDbContext<EvoAppDbContext> db, ILogger<TplTestService> logger, IServiceScopeFactory scopeFactory)
//         {
//             _db = db;
//             _logger = logger;
//             _scopeFactory = scopeFactory;
//         }
//
//         public async Task ExecuteInsertRangeParallel(TplTestModelDto data)
//         {
//             ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxIoThreads);
//             Console.WriteLine($"Maximum worker threads: {maxWorkerThreads}");
//             Console.WriteLine($"Maximum I/O threads: {maxIoThreads}");
//             
//             _timer.Restart();
//
//             await ThreadingHelper.ParallelizeAsync(async (index) =>
//             {
//                 // Creating a sub-scope allows separate DB contexts to be used
//                 // (even if BookContext is registered with Scoped lifetime, unlike Fix #1).
//                 using (var scope = _scopeFactory.CreateScope())
//                 {
//                     var dbService = scope.ServiceProvider.GetRequiredService<IEntityDbContext<EvoAppDbContext>>();
//                     var tplModel = new TplTestModel($"SerialNumber:{index}", $"TestContent {index}");
//                     
//                     // while (Interlocked.Decrement(ref count) >= 0)
//                     using (await AsyncLocker.LockForResource<TplTestModel>())
//                     {
//                         await dbService.Create(tplModel);
//                     }
//                 }
//             }, ParallelizationFactor);
//
//             // var tasks = new Task[ParallelizationFactor];
//             // for (int i = 0; i < ParallelizationFactor; i++)
//             // {
//             //     tasks[i] = Task.Run(async () =>
//             //     {
//             //         var tplModel = new TplTestModel($"SerialNumber:{i}", $"TestContent {i}");
//             //         using (await AsyncLocker.LockForResource<TplTestModel>())
//             //         {
//             //             await _db.Create(tplModel);
//             //         }
//             //     });
//             // }
//     
//             _timer.Stop();
//             _logger.LogTimeElapsed($"{nameof(ExecuteInsertRangeParallel)} Finished:", _timer.Elapsed);
//         }
//
//         public async Task ExecuteUpdateRangeParallel(int count)
//         {
//             _timer.Restart();
//             
//             const int countToTake = 10;
//             int padding = 0;
//             await ThreadingHelper.ParallelizeAsync(async (index) =>
//             {
//                 // Creating a sub-scope allows separate DB contexts to be used
//                 // (even if BookContext is registered with Scoped lifetime, unlike Fix #1).
//                 using (var scope = _scopeFactory.CreateScope())
//                 {
//                     var dbService = scope.ServiceProvider.GetRequiredService<IEntityDbContext<EvoAppDbContext>>();
//                     var tplModel = new TplTestModel($"SerialNumber:{index}", $"TestContent {index}");
//                     var tplTestModels = _db.List<TplTestModel>().Take(count);
//                     
//                     // while (Interlocked.Decrement(ref count) >= 0)
//                     using (await AsyncLocker.LockForResource<TplTestModel>())
//                     {
//                         await dbService.Create(tplModel);
//                     }
//                     Interlocked.Add(ref padding, countToTake);
//                 }
//             }, count / countToTake);
//     
//             _timer.Stop();
//             _logger.LogTimeElapsed($"{nameof(ExecuteUpdateRangeParallel)} Finished:", _timer.Elapsed);
//         }
//
//         public async Task ExecuteInsertRangeAsync(TplTestModelDto data)
//         {
//             _timer.Restart();
//
//             for (int i = 0; i < ParallelizationFactor; i++)
//             {
//                 var tplModel = new TplTestModel($"SerialNumber:{i}", $"TestContent {i}");
//                 await _db.Create(tplModel);
//             }
//     
//             _timer.Stop();
//             _logger.LogTimeElapsed($"{nameof(ExecuteInsertRangeAsync)} Finished:", _timer.Elapsed);
//         }
//
//         public async Task ExecuteUpdateRangeAsync(int count)
//         {
//             _timer.Restart();
//
//             var tplTestModels = _db.List<TplTestModel>().Take(count);
//             foreach (var tplTestModel in tplTestModels)
//             {
//                 tplTestModel.RandomUpdate();
//                 await _db.Update<TplTestModel>(tplTestModel);
//             }
//     
//             _timer.Stop();
//             _logger.LogTimeElapsed($"{nameof(ExecuteUpdateRangeAsync)} Finished:", _timer.Elapsed);
//         }
//     }
// }