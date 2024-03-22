using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace NetVisionProc.Data
{
    public class SeedRunner
    {
        private readonly Stopwatch _timer = new();
        private readonly IServiceProvider _services;

        public SeedRunner(IServiceProvider services)
        {
            _services = services;
        }

        public async Task ApplyAll()
        {
            // await ApplySeedAsync<CountrySeed>();
        }

        private async Task ApplySeedAsync<TSeedAsync>()
            where TSeedAsync : IDbSeedApplyAsync
        {
            _timer.Restart();
            var seed = _services.GetRequiredService<TSeedAsync>();
            await seed.Apply();
            _timer.Stop();
        }

        private void ApplySeed<TSeed>()
            where TSeed : IDbSeedApply
        {
            _timer.Restart();
            var seed = _services.GetRequiredService<TSeed>();
            seed.Apply();
            _timer.Stop();
        }
    }
}